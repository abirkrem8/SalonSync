using HairApplication.Logic.Shared;
using HairApplication.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairApplication.Logic.AppointmentConfirmation
{
    public class AppointmentConfirmationHandler
    {
        private FirestoreProvider _firestoreProvider;
        private CancellationToken _cancellationToken;

        public AppointmentConfirmationHandler(FirestoreProvider firestoreProvider)
        {
            _firestoreProvider = firestoreProvider;
            _cancellationToken = new CancellationTokenSource().Token;
        }

        public AppointmentConfirmationResult Handle(AppointmentConfirmationItem appointmentConfirmationItem)
        {
            AppointmentConfirmationResult result = new AppointmentConfirmationResult();

            AppointmentConfirmationValidator validator = new AppointmentConfirmationValidator();
            var validationResult = validator.Validate(appointmentConfirmationItem);

            if (!validationResult.IsValid)
            {
                // There was an error in validation, quit now
                // log the error
                return result;
            }

            // Successful validation, do the handling
            // Get the client information for a returning client based on the phone number
            // If it is supposed to be a new client, the phone number should be unique to the new client
            // Else this is a new client that will be created later after confirmation!
            Client client;
            bool clientFound=false;

            var clientsMatchingPhoneNumber = _firestoreProvider.WhereEqualTo<Client>("PhoneNumber", appointmentConfirmationItem.ClientPhoneNumber, _cancellationToken).Result.ToList();
            if (clientsMatchingPhoneNumber.Any() && appointmentConfirmationItem.IsNewClient)
            {
                // log to the user that there is already a client under that phone number
                client = clientsMatchingPhoneNumber[0];
                clientFound = true;
            }
            else if (clientsMatchingPhoneNumber.Any() && !appointmentConfirmationItem.IsNewClient)
            {
                // Found the existing client
                client = clientsMatchingPhoneNumber[0];
                clientFound = true;
            }
            else
            {
                // This will be a new client
                client = new Client()
                {
                    FirstName = appointmentConfirmationItem.ClientFirstName,
                    LastName = appointmentConfirmationItem.ClientLastName,
                    PhoneNumber = appointmentConfirmationItem.ClientPhoneNumber
                };
            }

            HairStylist stylist = _firestoreProvider.Get<HairStylist>(appointmentConfirmationItem.SelectedStylist, _cancellationToken).Result;

            result = new AppointmentConfirmationResult()
            {
                HairStylistFirstName = stylist.FirstName,
                HairStylistLastName = stylist.LastName,
                HairStylistId = stylist.Id,
                ClientFirstName = client.FirstName,
                ClientLastName = client.LastName,
                ClientPhoneNumber = client.PhoneNumber,
                ClientId = client.Id,
                ExistingClientFound = clientFound,
                DateTimeOfAppointment = new DateTime(appointmentConfirmationItem.DateOfAppointment.Year, appointmentConfirmationItem.DateOfAppointment.Month,
                    appointmentConfirmationItem.DateOfAppointment.Day, appointmentConfirmationItem.TimeOfAppointment.Hour, appointmentConfirmationItem.TimeOfAppointment.Minute,
                    appointmentConfirmationItem.TimeOfAppointment.Second)
            };


            return result;
        }
    }
}
