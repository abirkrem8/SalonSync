using SalonSync.Logic.AppointmentSchedule;
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

namespace SalonSync.GenerateData
{
    public interface IAppointmentScheduleService
    {
        int Run(int numberOfDaysToSchedule, bool historical);
    }

    public class AppointmentScheduleService : IAppointmentScheduleService
    {
        private AppointmentScheduleHandler _appointmentScheduleHandler;
        private LoadAppointmentScheduleFormHandler _loadAppointmentScheduleFormHandler;
        private FirestoreProvider _firestoreProvider;
        private ILogger<AppointmentScheduleService> _logger;
        private Random _random;
        private CancellationToken _cancellationToken;
        private const int MAX_APPOINTMENTS = 2;
        private const int APPOINTMENT_LENGTH = 2;

        public AppointmentScheduleService(ILogger<AppointmentScheduleService> logger,
            AppointmentScheduleHandler appointmentScheduleHandler, LoadAppointmentScheduleFormHandler loadAppointmentScheduleFormHandler,
            FirestoreProvider firestoreProvider)
        {
            _appointmentScheduleHandler = appointmentScheduleHandler;
            _loadAppointmentScheduleFormHandler = loadAppointmentScheduleFormHandler;
            _firestoreProvider = firestoreProvider;
            _logger = logger;
            _random = new Random();
            _cancellationToken = new CancellationTokenSource().Token;
        }


        public int Run(int numberOfDaysToSchedule, bool historical)
        {
            _logger.LogInformation("In the Appointment Scheduling Service!");

            // First grab all of the clients
            var listOfClients = _firestoreProvider.GetAll<Client>(_cancellationToken).Result.ToList();

            var listOfStylists = _firestoreProvider.GetAll<HairStylist>(_cancellationToken).Result.ToList();

            var availableAppointmentsResult = _loadAppointmentScheduleFormHandler.Handle(new LoadAppointmentScheduleFormItem());
            if (availableAppointmentsResult.LoadAppointmentScheduleFormResultStatus != LoadAppointmentScheduleFormResultStatus.Success)
            {
                _logger.LogError(string.Format("Unable to load available appointments for salon. {0}", availableAppointmentsResult.LoadAppointmentScheduleFormResultErrors[0].Message));
                return -1;
            }

            var availableAppointmentsThisMonth = availableAppointmentsResult.AvailableAppointmentsForEachStylist;


            foreach (var stylist in listOfStylists)
            {
                // schedule up to two appointments per day
                // grab all appointments that are available for the stylist
                var thisStylistsAvailableAppointments = availableAppointmentsThisMonth[stylist.Id];

                // For each date, schedule up to two appointments
                for (int daysFromToday = 0; daysFromToday < numberOfDaysToSchedule; daysFromToday++)
                {
                    DateTime dayToSchedule = DateTime.Now.AddDays(daysFromToday).Date;
                    var daysAvailableAppointments = thisStylistsAvailableAppointments.Where(x => x.Date == dayToSchedule).ToList();

                    // select two random available appointments to schedule and make sure they don't overlap.
                    TimePeriodCollection aptTimeCollection = new TimePeriodCollection();
                    if (daysAvailableAppointments.Count > 1)
                    {
                        while (aptTimeCollection.Count < MAX_APPOINTMENTS && daysAvailableAppointments.Count > 0)
                        {
                            var randomSelectedApt = daysAvailableAppointments[_random.Next(0, daysAvailableAppointments.Count)];
                            var aptAsTimeRange = new TimeRange(TimeTrim.Hour(dayToSchedule, randomSelectedApt.Hour, randomSelectedApt.Minute),
                                                TimeTrim.Hour(dayToSchedule, randomSelectedApt.Hour + APPOINTMENT_LENGTH, randomSelectedApt.Minute));

                            if (aptTimeCollection.Count == 0 || !aptTimeCollection.HasOverlapPeriods(aptAsTimeRange))
                            {
                                aptTimeCollection.Add(aptAsTimeRange);
                                daysAvailableAppointments.Remove(randomSelectedApt);
                            } else
                            {
                                // overlaps so we don't want to pick it again
                                daysAvailableAppointments.Remove(randomSelectedApt);
                            }
                        }
                    }

                    var aptEnumerator = aptTimeCollection.GetEnumerator();
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
                            PhoneNumber = client.PhoneNumber
                        };

                        _logger.LogInformation(String.Format("Scheduling {0} {1} for an appointment at {2} {3}!", 
                            scheduleItem.FirstName, scheduleItem.LastName, scheduleItem.DateOfAppointment.ToShortDateString(),
                            scheduleItem.TimeOfAppointment.ToShortTimeString()));

                        var result = _appointmentScheduleHandler.Handle(scheduleItem);
                        if (result.AppointmentScheduleResultStatus != AppointmentScheduleResultStatus.Success)
                        {
                            // log error!
                            _logger.LogError(string.Format("There was an error! {0}", result.AppointmentScheduleResultErrors.First().Message));
                            continue;
                        }
                    }
                }
            }

            return 0;
        }

    }
}
