using Itenso.TimePeriod;
using Microsoft.Extensions.Logging;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.GetAvailableAppointments
{
    public class GetAvailableAppointmentsHandler
    {
        private const int NUMBER_OF_DAYS_AVAILABLE_TO_SCHEDULE = 30;
        private const int OPENING_HOUR = 8;
        private const int CLOSING_HOUR = 18;
        private const int HOUR_LENGTH_OF_APPOINTMENT = 2;
        private const int MINUTE_INTERVAL = 60;
        private ILogger<GetAvailableAppointmentsHandler> _logger;
        private FirestoreProvider _firestoreProvider;
        private CancellationToken _cancellationToken;

        public GetAvailableAppointmentsHandler(ILogger<GetAvailableAppointmentsHandler> logger, FirestoreProvider firestoreProvider)
        {
            _logger = logger;
            _firestoreProvider = firestoreProvider;
            _cancellationToken = new CancellationTokenSource().Token;
        }

        public GetAvailableAppointmentsResult Handle(GetAvailableAppointmentsItem getAvailableAppointmentsItem)
        {
            GetAvailableAppointmentsResult result = new GetAvailableAppointmentsResult();

            GetAvailableAppointmentsValidator validator = new GetAvailableAppointmentsValidator();
            var validationResult = validator.Validate(getAvailableAppointmentsItem);

            if (!validationResult.IsValid)
            {
                // There was an error in validation, quit now
                // log the error
                return result;
            }

            // Successful validation, do the handling
            var stylist = _firestoreProvider.Get<HairStylist>(getAvailableAppointmentsItem.StylistId, _cancellationToken).Result;
            var stylistRef = _firestoreProvider.ConvertIdToReference<HairStylist>(getAvailableAppointmentsItem.StylistId);
            // Grab all appointments with this stylist for the current day
            var existingAppointments = _firestoreProvider.WhereEqualTo<Appointment>("HairStylist", stylistRef, _cancellationToken).Result.ToList();
            List<DateTime> listOfTimes = new List<DateTime>();

            for (DateTime currentDay = getAvailableAppointmentsItem.StartDate; 
                currentDay <= getAvailableAppointmentsItem.EndDate; 
                currentDay = currentDay.AddDays(1))
            {
                // Create a time collection for the day to schedule
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

                _logger.LogInformation(string.Format("Stylist {0} {1} has {2} appointments on {3}.", stylist.FirstName, stylist.LastName, aptTimeCollection.Count, currentDay.ToShortDateString()));

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
                    _logger.LogError(string.Format("There are overlaps in the schedule for stylist: {0} on {1}!", stylist.FirstName, currentDay.ToShortDateString()));
                    continue;
                }
            }


            result.AvailableAppointments = listOfTimes;
            result.GetAvailableAppointmentsResultStatus = GetAvailableAppointmentsResultStatus.Success;
            return result;
        }
    }
}
