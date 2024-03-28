using Google.Cloud.Firestore;
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
        private const int NUMBER_OF_DAYS_AVAILABLE_TO_SCHEDULE = 14;
        private const int OPENING_HOUR = 8;
        private const int CLOSING_HOUR = 18;
        private const int HOUR_LENGTH_OF_APPOINTMENT = 2;
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
            



            return result;
        }

        private Dictionary<string, List<DateTime>> GetAvailableAppointmentTimes()
        {
            var stylists = _firestoreProvider.GetAll<HairStylist>(_cancellationToken).Result.ToList();
            Dictionary<string, List<DateTime>> availableTimes = new Dictionary<string, List<DateTime>>();


            stylists.ForEach(sty =>
            {
                List<DateTime> timeList = new List<DateTime>();
                DocumentReference stylistRef = _firestoreProvider.ConvertIdToReference<HairStylist>(sty.Id);
                List<DateTime> timesOfScheduledAppointments = _firestoreProvider.WhereEqualTo<Appointment>("HairStylist", stylistRef, _cancellationToken).Result.ToList()
                                                .Select(x => x.StartTimeOfAppointment.ToDateTime()).ToList()
                                                .Where(x => x > DateTime.Now && x.Date <= DateTime.Now.AddDays(NUMBER_OF_DAYS_AVAILABLE_TO_SCHEDULE).Date).ToList()
                                                .OrderBy(x => x).ToList();
                DateTime loopDate = DateTime.Now.Date;

                for (int i = 0; i < timesOfScheduledAppointments.Count - 1; i++)
                {
                    if (timesOfScheduledAppointments[i].Date > loopDate)
                    {
                        loopDate = timesOfScheduledAppointments[i].Date;
                        if (timesOfScheduledAppointments[i].Hour - HOUR_LENGTH_OF_APPOINTMENT )

                    }
                }

            });

            return availableTimes;
        }
    }
}
