using Microsoft.Extensions.Logging;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using SalonSync.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.AddAppointmentNotes
{
    public class AddAppointmentNotesHandler
    {
        private ILogger<AddAppointmentNotesHandler> _logger;
        private FirestoreProvider _firestoreProvider;
        private CancellationToken _cancellationToken;

        public AddAppointmentNotesHandler(ILogger<AddAppointmentNotesHandler> logger, FirestoreProvider firestoreProvider)
        {
            _logger = logger;
            _firestoreProvider = firestoreProvider;
            _cancellationToken = new CancellationTokenSource().Token;
        }

        public AddAppointmentNotesResult Handle(AddAppointmentNotesItem addAppointmentNotesItem)
        {
            AddAppointmentNotesResult result = new AddAppointmentNotesResult();

            AddAppointmentNotesValidator validator = new AddAppointmentNotesValidator();
            var validationResult = validator.Validate(addAppointmentNotesItem);

            if (!validationResult.IsValid)
            {
                // There was an error in validation, quit now
                // log the error
                string error = string.Format("Validation Error: {0}", validationResult.Errors.FirstOrDefault());
                _logger.LogError(error);
                result.AddAppointmentNotesResultStatus = AddAppointmentNotesResultStatus.ValidationError;
                result.AddAppointmentNotesResultErrors.Add(new Error { Message = error });
                return result;
            }

            // Successful validation, do the handling
            try
            {
                // Grab appointment from db
                var appointment = _firestoreProvider.Get<Appointment>(addAppointmentNotesItem.AppointmentId, _cancellationToken).Result;
                if (appointment.AppointmentNotes == null)
                {
                    appointment.AppointmentNotes = new List<string>();
                }
                appointment.AppointmentNotes.Add(addAppointmentNotesItem.NoteText);
                // update db
                _firestoreProvider.AddOrUpdate(appointment, _cancellationToken).Wait();

                _logger.LogInformation("Successfully added note to appointment!");
                result.AddAppointmentNotesResultStatus = AddAppointmentNotesResultStatus.Success;
                return result;
            }
            catch (Exception ex)
            {
                // Exceptions with the Firebase DB
                // log the error
                string error = string.Format("Database Error! {0}", ex.Message);
                _logger.LogError(error);
                result.AddAppointmentNotesResultStatus = AddAppointmentNotesResultStatus.DatabaseError;
                result.AddAppointmentNotesResultErrors.Add(new Error { Message = error });
                return result;
            }
        }
    }
}
