using Google.Cloud.Firestore;
using Itenso.TimePeriod;
using Microsoft.Extensions.Logging;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairApplication.Logic.LoadAppointmentScheduleForm
{
    public class LoadAppointmentScheduleFormHandler
    {
        private ILogger<LoadAppointmentScheduleFormHandler> _logger;
        private const int NUMBER_OF_DAYS_AVAILABLE_TO_SCHEDULE = 30;
        private const int OPENING_HOUR = 8;
        private const int CLOSING_HOUR = 18;
        private const int HOUR_LENGTH_OF_APPOINTMENT = 2;
        private const int MINUTE_INTERVAL = 60;
        private FirestoreProvider _firestoreProvider;
        private CancellationToken _cancellationToken;

        public LoadAppointmentScheduleFormHandler(ILogger<LoadAppointmentScheduleFormHandler> logger,
            FirestoreProvider firestoreProvider)
        {
            _logger = logger;
            _firestoreProvider = firestoreProvider;
            _cancellationToken = new CancellationTokenSource().Token;
        }

        public LoadAppointmentScheduleFormResult Handle(LoadAppointmentScheduleFormItem LoadAppointmentScheduleFormItem)
        {
            LoadAppointmentScheduleFormResult result = new LoadAppointmentScheduleFormResult();

            LoadAppointmentScheduleFormValidator validator = new LoadAppointmentScheduleFormValidator();
            var validationResult = validator.Validate(LoadAppointmentScheduleFormItem);

            if (!validationResult.IsValid)
            {
                // There was an error in validation, quit now
                // log the error
                return result;
            }

            // Successful validation, do the handling
            result.HairStylists = _firestoreProvider.GetAll<HairStylist>(_cancellationToken).Result.ToList();

            result.AvailableAppointmentsForEachStylist = GetAvailableAppointmentTimes(result.HairStylists);
            result.LoadAppointmentScheduleFormResultStatus = LoadAppointmentScheduleFormResultStatus.Success;
            return result;
        }

        private Dictionary<string, List<DateTime>> GetAvailableAppointmentTimes(List<HairStylist> stylists)
        {
            Dictionary<string, List<DateTime>> availableTimes = new Dictionary<string, List<DateTime>>();

            foreach (HairStylist sty in stylists)
            {
                var stylistRef = _firestoreProvider.ConvertIdToReference<HairStylist>(sty.Id);
                // Grab all appointments with this stylist for the current day
                var existingAppointments = _firestoreProvider.WhereEqualTo<Appointment>("HairStylist", stylistRef, _cancellationToken).Result.ToList();
                List<DateTime> listOfTimes = new List<DateTime>();

                for (int dayNum = 0; dayNum < NUMBER_OF_DAYS_AVAILABLE_TO_SCHEDULE; dayNum++)
                {
                    // Create a time collection for the day to schedule
                    DateTime currentDay = DateTime.Now.AddDays(dayNum);
                    TimePeriodCollection aptTimeCollection = new TimePeriodCollection();
                    var startOfDay = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, OPENING_HOUR, 0, 0);
                    var endOfDay = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, CLOSING_HOUR, 0, 0);

                    // Add all appointments for the day to the time collection
                    existingAppointments.Where(x => x.StartTimeOfAppointment.ToDateTime().ToLocalTime().Date == currentDay.Date).ToList().ForEach(a =>
                    {
                        var s = a.StartTimeOfAppointment.ToDateTime().ToLocalTime();
                        var e = a.EndTimeOfAppointment.ToDateTime().ToLocalTime();
                        aptTimeCollection.Add(new TimeRange(TimeTrim.Hour(currentDay, s.Hour, s.Minute),
                                        TimeTrim.Hour(currentDay, e.Hour, e.Minute)));
                    });

                    _logger.LogInformation(string.Format("Stylist {0} {1} has {2} appointments on {3}.", sty.FirstName, sty.LastName, aptTimeCollection.Count, currentDay.ToShortDateString()));

                    // Handle the time collection
                    if (!aptTimeCollection.HasOverlaps())
                    {
                        TimeRange testTime = new TimeRange(startOfDay, new TimeSpan(HOUR_LENGTH_OF_APPOINTMENT, 0, 0));
                        bool reachedEndOfDay = false;
                        while (!reachedEndOfDay)
                        {
                            if (!aptTimeCollection.HasOverlapPeriods(testTime))
                            {
                                listOfTimes.Add(testTime.Start);
                                _logger.LogInformation(string.Format("Adding time period {0} to the list of available times.", testTime.ToString()));
                            }
                            else
                            {
                                _logger.LogInformation(string.Format("There are overlap periods for {0}! The periods that exist are {1}.", testTime.ToString(), aptTimeCollection.OverlapPeriods(testTime).ToString()));
                            }
                            if (endOfDay <= testTime.End)
                            {
                                reachedEndOfDay = true;
                            }
                            testTime.Move(new TimeSpan(0, MINUTE_INTERVAL, 0));
                        }
                    }
                    else
                    {
                        // it is already invalid!
                        // log error
                        _logger.LogError(String.Format("There are overlaps in the schedule for stylist: {0} on {1}!", sty.FirstName, currentDay.ToShortDateString()));
                        continue;
                    }
                }
                availableTimes.Add(sty.Id, listOfTimes);
            }

            return availableTimes;
        }
    }
}
