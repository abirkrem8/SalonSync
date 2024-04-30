using Google.Cloud.Firestore;
using Itenso.TimePeriod;
using Microsoft.Extensions.Logging;
using SalonSync.Logic.GetAvailableAppointments;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using SalonSync.Models.Shared;
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
            _logger.LogInformation("Loading Appointment Scheduling Form");
            LoadAppointmentScheduleFormResult result = new LoadAppointmentScheduleFormResult();

            LoadAppointmentScheduleFormValidator validator = new LoadAppointmentScheduleFormValidator();
            var validationResult = validator.Validate(LoadAppointmentScheduleFormItem);

            if (!validationResult.IsValid)
            {
                // There was an error in validation, quit now
                // log the error
                string error = string.Format("Validation Error: {0}", validationResult.Errors.FirstOrDefault());
                _logger.LogError(error);
                result.LoadAppointmentScheduleFormResultStatus = LoadAppointmentScheduleFormResultStatus.ValidationError;
                result.LoadAppointmentScheduleFormResultErrors.Add(new Error { Message = error });
                return result;
            }

            // Successful validation, do the handling
            try
            {
                result.HairStylists = _firestoreProvider.GetAll<HairStylist>(_cancellationToken).Result.ToList();

                Dictionary<string, List<DateTime>> availableTimes = new Dictionary<string, List<DateTime>>();

                // For each stylist, grab all of the available appointment times between now and 30 days in the future
                foreach (HairStylist sty in result.HairStylists)
                {
                    var item = new GetAvailableAppointmentsItem()
                    {
                        StylistId = sty.Id,
                        StartDate = DateTime.Now.Date,
                        EndDate = DateTime.Now.Date.AddDays(NUMBER_OF_DAYS_AVAILABLE_TO_SCHEDULE),
                    };
                    // Call the handler to get the available appointment times for this stylist
                    var availableAppointmentsResult = _getAvailableAppointmentsHandler.Handle(item);
                    if (availableAppointmentsResult.GetAvailableAppointmentsResultStatus != GetAvailableAppointmentsResultStatus.Success)
                    {
                        // There was an error in grabbing available times for a stylist, quit now
                        string error = string.Format("Error Grabbing Available Appointment Times: {0} - {1}", availableAppointmentsResult.GetAvailableAppointmentsResultStatus.ToString(),availableAppointmentsResult.GetAvailableAppointmentsResultErrors.FirstOrDefault());
                        _logger.LogError(error);
                        result.LoadAppointmentScheduleFormResultStatus = LoadAppointmentScheduleFormResultStatus.AvailableAppointmentsError;
                        result.LoadAppointmentScheduleFormResultErrors.Add(new Error { Message = error });
                        return result;
                    }
                    // Store times in a dictionary with a key of stylist ID
                    availableTimes.Add(sty.Id, availableAppointmentsResult.AvailableAppointments);
                }

                result.AvailableAppointmentsForEachStylist = availableTimes;
                result.LoadAppointmentScheduleFormResultStatus = LoadAppointmentScheduleFormResultStatus.Success;
                _logger.LogInformation("Successfully loaded Appointment Scheduling Form information");
                return result;
            }
            catch (Exception ex)
            {
                // Exceptions with the Firebase DB
                // log the error
                string error = string.Format("Database Error! {0}", ex.Message);
                _logger.LogError(error);
                result.LoadAppointmentScheduleFormResultStatus = LoadAppointmentScheduleFormResultStatus.DatabaseError;
                result.LoadAppointmentScheduleFormResultErrors.Add(new Error { Message = error });
                return result;
            }
        }
    }
}
