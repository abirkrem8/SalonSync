using Itenso.TimePeriod;
using Microsoft.Extensions.Logging;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using SalonSync.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.GetAvailableAppointments
{
    public class GetAvailableAppointmentsHandler
    {
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
            // GOAL: Return a list of datetimes of available appointments for the given stylist between the start and end times
            GetAvailableAppointmentsResult result = new GetAvailableAppointmentsResult();

            GetAvailableAppointmentsValidator validator = new GetAvailableAppointmentsValidator();
            var validationResult = validator.Validate(getAvailableAppointmentsItem);

            if (!validationResult.IsValid)
            {
                // There was an error in validation, quit now
                // log the error
                string error = string.Format("Validation Error: {0}", validationResult.Errors.FirstOrDefault());
                _logger.LogError(error);
                result.GetAvailableAppointmentsResultStatus = GetAvailableAppointmentsResultStatus.ValidationError;
                result.GetAvailableAppointmentsResultErrors.Add(new Error { Message = error });
                return result;
            }

            // Successful validation, do the handling
            try
            {
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
                        // log error, move to the next day
                        _logger.LogError(string.Format("There are overlaps in the schedule for stylist: {0} on {1}!", stylist.FirstName, currentDay.ToShortDateString()));
                        continue;
                    }
                }
                // return list of available appointment times for the given stylist between the start and end dates
                result.AvailableAppointments = listOfTimes;
                result.GetAvailableAppointmentsResultStatus = GetAvailableAppointmentsResultStatus.Success;
                return result;
            }
            catch (Exception ex)
            {
                // Exceptions with the Firebase DB
                // log the error
                string error = string.Format("Database Error! {0}", ex.Message);
                _logger.LogError(error);
                result.GetAvailableAppointmentsResultStatus = GetAvailableAppointmentsResultStatus.DatabaseError;
                result.GetAvailableAppointmentsResultErrors.Add(new Error { Message = error });
                return result;
            }
        }
    }
}
