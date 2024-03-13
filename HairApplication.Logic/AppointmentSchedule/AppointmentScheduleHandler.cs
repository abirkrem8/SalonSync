using HairApplication.Logic.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairApplication.Logic.AppointmentSchedule
{
    public class AppointmentScheduleHandler
    {
        private FirestoreProvider _firestoreProvider;

        public AppointmentScheduleHandler(FirestoreProvider firestoreProvider)
        {
            _firestoreProvider = firestoreProvider;
        }

        public AppointmentScheduleResult Handle(AppointmentScheduleItem appointmentScheduleItem)
        {
            AppointmentScheduleResult result = new AppointmentScheduleResult();
            
            AppointmentScheduleValidator validator = new AppointmentScheduleValidator();
            var validationResult = validator.Validate(appointmentScheduleItem);

            if (!validationResult.IsValid)
            {
                // There was an error in validation, quit now
                // log the error
                return result;
            }

            // Successful validation, do the handling
            
            // Grab the stylist ID


            // New client? Add to DB and grab ID


            // Create Appointment object and add to DB, grab ID


            // Update Appointment DB with appointment
            // Update Client DB with appointment
            // Update Stylist DB with appointment


            return result;
        }
    }
}
