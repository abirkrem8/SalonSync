using Google.Cloud.Firestore;
using Itenso.TimePeriod;
using Microsoft.Extensions.Logging;
using SalonSync.Logic.GetAvailableAppointments;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.Load.LoadAppointmentScheduleForm
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
        private GetAvailableAppointmentsHandler _getAvailableAppointmentsHandler;

        public LoadAppointmentScheduleFormHandler(ILogger<LoadAppointmentScheduleFormHandler> logger,
            FirestoreProvider firestoreProvider, GetAvailableAppointmentsHandler getAvailableAppointmentsHandler)
        {
            _logger = logger;
            _firestoreProvider = firestoreProvider;
            _cancellationToken = new CancellationTokenSource().Token;
            _getAvailableAppointmentsHandler = getAvailableAppointmentsHandler;
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
                var item = new GetAvailableAppointmentsItem()
                {
                    StylistId = sty.Id,
                    StartDate = DateTime.Now.Date,
                    EndDate = DateTime.Now.Date.AddDays(NUMBER_OF_DAYS_AVAILABLE_TO_SCHEDULE),
                };
                var result = _getAvailableAppointmentsHandler.Handle(item);
                if (result.GetAvailableAppointmentsResultStatus != GetAvailableAppointmentsResultStatus.Success)
                {
                    //handle error
                    // return
                }
                availableTimes.Add(sty.Id, result.AvailableAppointments);
            }

            return availableTimes;
        }
    }
}
