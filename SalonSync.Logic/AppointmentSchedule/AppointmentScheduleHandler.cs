using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using SalonSync.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.AppointmentSchedule
{
    public class AppointmentScheduleHandler
    {
        private ILogger<AppointmentScheduleHandler> _logger;
        private FirestoreProvider _firestoreProvider;
        private CancellationToken _cancellationToken;

        public AppointmentScheduleHandler(FirestoreProvider firestoreProvider, ILogger<AppointmentScheduleHandler> logger)
        {
            _logger = logger;
            _firestoreProvider = firestoreProvider;
            _cancellationToken = new CancellationTokenSource().Token;
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
                result.AppointmentScheduleResultStatus = AppointmentScheduleResultStatus.ValidationError;
                return result;
            }

            // Successful validation, do the handling
            HairStylist hairStylist;
            DocumentReference stylistReference;
            Client client;
            DocumentReference clientReference;
            Appointment appointment;
            DocumentReference appointmentReference;


            // Grab the stylist reference
            hairStylist = _firestoreProvider.Get<HairStylist>(appointmentScheduleItem.HairStylistId, _cancellationToken).Result;
            stylistReference = _firestoreProvider.ConvertIdToReference<HairStylist>(hairStylist.Id);

            // New client? Add to DB and grab Reference
            if (appointmentScheduleItem.IsNewClient)
            {
                client = new Client(appointmentScheduleItem.FirstName, appointmentScheduleItem.LastName,
                    appointmentScheduleItem.PhoneNumber, Models.Enums.HairTexture.UNKNOWN, Models.Enums.HairLength.UNKNOWN);

                clientReference = _firestoreProvider.AddOrUpdate(client, _cancellationToken).Result;
            }
            else
            {
                // grab all clients and find the ONE with the same phone number
                var clients = _firestoreProvider.GetAll<Client>(_cancellationToken).Result.ToList();

                var matchingClients = clients.Where(x => x.PhoneNumber.Equals(appointmentScheduleItem.PhoneNumber)).ToList();
                if (matchingClients.Count() < 1)
                {
                    // throw error!
                    // log error and send it back to view model
                    string error = string.Format("An existing client was not found under the phone number: {0}", appointmentScheduleItem.PhoneNumber);
                    _logger.LogError(error);
                    result.AppointmentScheduleResultStatus = AppointmentScheduleResultStatus.NoExistingClient;
                    result.AppointmentScheduleResultErrors.Add(new Error { Message = error });
                    return result;
                }
                client = matchingClients[0];
                clientReference = _firestoreProvider.ConvertIdToReference<Client>(client.Id);
            }

            // Create Appointment object and add to DB, grab ID
            appointment = new Appointment(stylistReference, clientReference, hairStylist.FirstName,hairStylist.LastName, client.FirstName, client.LastName, client.PhoneNumber, appointmentScheduleItem.DateOfAppointment, appointmentScheduleItem.TimeOfAppointment);
            appointmentReference= _firestoreProvider.AddOrUpdate(appointment, _cancellationToken).Result;

            result.ClientFullName = String.Concat(client.FirstName, " ", client.LastName);
            result.StylistFullName = String.Concat(hairStylist.FirstName, " ", hairStylist.LastName);
            result.TimeOfAppointment = appointment.StartTimeOfAppointment.ToDateTime().ToLocalTime();
            result.AppointmentScheduleResultStatus = AppointmentScheduleResultStatus.Success;

            return result;
        }
    }
}
