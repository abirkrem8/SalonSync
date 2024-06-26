﻿using SalonSync.Logic.AppointmentSchedule;
using Microsoft.Extensions.Logging;
using SalonSync.Logic.AppointmentSchedule;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Itenso.TimePeriod;
using SalonSync.Logic.Load.LoadAppointmentScheduleForm;
using SalonSync.Models.Enums;
using SalonSync.Logic.AddAppointmentNotes;
using SalonSync.Logic.GetAvailableAppointments;
using HairApplication.Logic.CreateNewClient;

namespace SalonSync.GenerateData
{
    public interface IAppointmentScheduleService
    {
        int Run(CommandLineOptions options);
    }

    public class AppointmentScheduleService : IAppointmentScheduleService
    {
        private AppointmentScheduleHandler _appointmentScheduleHandler;
        private GetAvailableAppointmentsHandler _getAvailableAppointmentsHandler;
        private AddAppointmentNotesHandler _addAppointmentNotesHandler;
        private CreateNewClientHandler _createNewClientHandler;
        private FirestoreProvider _firestoreProvider;
        private ILogger<AppointmentScheduleService> _logger;
        private Random _random;
        private CancellationToken _cancellationToken;
        private const int MAX_APPOINTMENTS = 2;
        private const int APPOINTMENT_LENGTH = 2;

        public AppointmentScheduleService(ILogger<AppointmentScheduleService> logger,
            AppointmentScheduleHandler appointmentScheduleHandler,
            AddAppointmentNotesHandler addAppointmentNotesHandler, FirestoreProvider firestoreProvider,
            GetAvailableAppointmentsHandler getAvailableAppointmentsHandler,
            CreateNewClientHandler createNewClientHandler)
        {
            _appointmentScheduleHandler = appointmentScheduleHandler;
            _getAvailableAppointmentsHandler = getAvailableAppointmentsHandler;
            _addAppointmentNotesHandler = addAppointmentNotesHandler;
            _createNewClientHandler = createNewClientHandler;
            _firestoreProvider = firestoreProvider;
            _logger = logger;
            _random = new Random();
            _cancellationToken = new CancellationTokenSource().Token;
        }


        public int Run(CommandLineOptions options)
        {
            _logger.LogInformation("In the Appointment Scheduling Service!");

            if (options.CreateNewClients && options.NumberOfNewClientsToCreate > 0)
            {
                _logger.LogInformation(string.Format("Flag is set to create new clients!"));
                _logger.LogInformation(string.Format("Creating {0} new clients for the salon", options.NumberOfNewClientsToCreate));
                var createClientsResult = _createNewClientHandler.Handle(new CreateNewClientItem { NumberOfNewClientsToCreate = options.NumberOfNewClientsToCreate });
                if (createClientsResult.CreateNewClientResultStatus != CreateNewClientResultStatus.Success)
                {
                    // There was an error in validation, quit now
                    // log the error
                    string error = string.Format("Create New Client Error: {0}", createClientsResult.CreateNewClientResultErrors.FirstOrDefault());
                    _logger.LogError(error);
                    return -1;

                }
                // success, carry on
            }


            if (options.NumberOfDaysToSchedule != 0)
            {
                // options.ScheduleHistorically will not include scheduling for today
                // future scheduling will include today
                DateTime startDate = options.ScheduleHistorically ? DateTime.Now.AddDays(options.NumberOfDaysToSchedule * -1).Date : DateTime.Now.Date;
                DateTime endDate = options.ScheduleHistorically ? DateTime.Now.AddDays(-1).Date : DateTime.Now.AddDays(options.NumberOfDaysToSchedule).Date;

                _logger.LogInformation(string.Format("Scheduling appointments between {0} and {1}.", startDate.ToShortDateString(), endDate.ToShortDateString()));

                // First grab all of the clients
                var listOfClients = _firestoreProvider.GetAll<Client>(_cancellationToken).Result.ToList();

                var listOfStylists = _firestoreProvider.GetAll<HairStylist>(_cancellationToken).Result.ToList();

                foreach (var stylist in listOfStylists)
                {
                    // schedule up to two appointments per day
                    // grab all appointments that are available for the stylist
                    var item = new GetAvailableAppointmentsItem()
                    {
                        StylistId = stylist.Id,
                        StartDate = startDate,
                        EndDate = endDate
                    };
                    var availableAppointmentsResult = _getAvailableAppointmentsHandler.Handle(item);
                    if (availableAppointmentsResult.GetAvailableAppointmentsResultStatus != GetAvailableAppointmentsResultStatus.Success)
                    {
                        // log error and quit, FATAL
                        _logger.LogError(string.Format("There was an error while collection available appointment times! {0}", availableAppointmentsResult.GetAvailableAppointmentsResultErrors.First().Message));
                        return -2;
                    }
                    var thisStylistsAvailableAppointments = availableAppointmentsResult.AvailableAppointments;

                    // For each date, schedule up to two appointments
                    for (DateTime dayToSchedule = startDate;
                    dayToSchedule <= endDate;
                    dayToSchedule = dayToSchedule.AddDays(1))
                    {
                        var daysAvailableAppointments = thisStylistsAvailableAppointments.Where(x => x.Date == dayToSchedule).ToList();

                        // select two random available appointments to schedule on this day for the stylist and make sure they don't overlap.
                        #region Select Random Appointment Times
                        TimePeriodCollection aptTimeCollection = new TimePeriodCollection();
                        if (daysAvailableAppointments.Count > 1)
                        {
                            // Find two appointments randomly in available times to schedule that will not overlap each other
                            while (aptTimeCollection.Count < MAX_APPOINTMENTS && daysAvailableAppointments.Count > 0)
                            {
                                var randomSelectedApt = daysAvailableAppointments[_random.Next(0, daysAvailableAppointments.Count)];
                                var aptAsTimeRange = new TimeRange(TimeTrim.Hour(dayToSchedule, randomSelectedApt.Hour, randomSelectedApt.Minute),
                                                    TimeTrim.Hour(dayToSchedule, randomSelectedApt.Hour + APPOINTMENT_LENGTH, randomSelectedApt.Minute));

                                if (aptTimeCollection.Count == 0 || !aptTimeCollection.HasOverlapPeriods(aptAsTimeRange))
                                {
                                    aptTimeCollection.Add(aptAsTimeRange);
                                    daysAvailableAppointments.Remove(randomSelectedApt);
                                }
                                else
                                {
                                    // overlaps so we don't want to pick it again
                                    daysAvailableAppointments.Remove(randomSelectedApt);
                                }
                            }
                        } else if (daysAvailableAppointments.Count == 1)
                        {
                            var aptAsTimeRange = new TimeRange(TimeTrim.Hour(dayToSchedule, daysAvailableAppointments[0].Hour, daysAvailableAppointments[0].Minute),
                                                    TimeTrim.Hour(dayToSchedule, daysAvailableAppointments[0].Hour + APPOINTMENT_LENGTH, daysAvailableAppointments[0].Minute));
                            aptTimeCollection.Add(aptAsTimeRange);
                        } else
                        {
                            // no available appointments this day for the stylist
                            // move onto next day
                            continue;
                        }
                        #endregion

                        #region Schedule Appointments
                        var aptEnumerator = aptTimeCollection.GetEnumerator();

                        // Create each appointment that's been randomly selected
                        while (aptEnumerator.MoveNext())
                        {
                            var apt = aptEnumerator.Current;
                            var client = listOfClients[_random.Next(0, listOfClients.Count - 1)];

                            var scheduleItem = new AppointmentScheduleItem()
                            {
                                HairStylistId = stylist.Id,
                                IsNewClient = false,
                                FirstName = client.FirstName,
                                LastName = client.LastName,
                                DateOfAppointment = apt.Start.Date,
                                TimeOfAppointment = apt.Start,
                                PhoneNumber = client.PhoneNumber,
                                AppointmentType = (AppointmentType)_random.Next(0, 4),
                                HistoricalAppointmentSchedule = options.ScheduleHistorically
                            };

                            _logger.LogInformation(String.Format("Scheduling {0} {1} for an appointment at {2} {3} with {4} {5}!",
                                scheduleItem.FirstName, scheduleItem.LastName, scheduleItem.DateOfAppointment.ToShortDateString(),
                                scheduleItem.TimeOfAppointment.ToShortTimeString(), stylist.FirstName, stylist.LastName));

                            var result = _appointmentScheduleHandler.Handle(scheduleItem);
                            if (result.AppointmentScheduleResultStatus != AppointmentScheduleResultStatus.Success)
                            {
                                // log error! Not fatal, continue
                                _logger.LogError(string.Format("There was an error scheduling an appointment! {0}", result.AppointmentScheduleResultErrors.First().Message));
                                continue;
                            }


                            var appointmentNoteItem = new AddAppointmentNotesItem()
                            {
                                AppointmentId = result.AppointmentId,
                                NoteText = AppointmentNotes[scheduleItem.AppointmentType][_random.Next(0, 2)],
                            };
                            var result2 = _addAppointmentNotesHandler.Handle(appointmentNoteItem);
                            if (result2.AddAppointmentNotesResultStatus != AddAppointmentNotesResultStatus.Success)
                            {
                                // log error! Not fatal, continue
                                _logger.LogError(string.Format("There was an error adding a note to an existing appointment! {0}", result2.AddAppointmentNotesResultErrors.First().Message));
                                continue;
                            }
                            // end of appointment schedule while loop
                            #endregion
                        }
                        // end of date loop
                    }
                    // end of stylist loop
                }
                // end of if statement checking if there are any days to schedule
            }

            // Success
            return 0;
        }


        // Predetermined list of appointment notes to add
        private static Dictionary<AppointmentType, List<string>> AppointmentNotes = new Dictionary<AppointmentType, List<string>>()
        {
            {AppointmentType.HairCut, new List<string>
            {
                "Client wants to cut 2 inches off",
                "Client wants to cut short bangs",
                "Just a trim"
            }
            },
            {AppointmentType.NewClientConsultation, new List<string>
            {
                "Client wants to be seen by a curly hair specialist",
                "Client likes non-toxic shampoo",
                "Silent appointment"
            }
            },
            {AppointmentType.HighLight, new List<string>
            {
                "Client wants bright blonde highlights",
                "Money-pieces only",
                "Subtle highlights, nothing too bold"
            }
            },
            {AppointmentType.FullColor, new List<string>
            {
                "Natural warm-brown tones, small highlights",
                "Purple hair! Client wants a crazy color",
                "Update to previous color, just a root touch-up"
            }
            },
            {AppointmentType.Extensions, new List<string>
            {
                "12 inch extensions, matching root",
                "Blonde extensions, 16 inches",
                "Dark black extensions, 24 inches"
            }
            },
        };

    }
}
