using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.AppointmentConfirmation
{
    public class AppointmentConfirmationHandler
    {
        private ILogger<AppointmentConfirmationHandler> _logger;
        private FirestoreProvider _firestoreProvider;
        private CancellationToken _cancellationToken;

        public AppointmentConfirmationHandler(ILogger<AppointmentConfirmationHandler> logger, FirestoreProvider firestoreProvider)
        {
            _logger = logger;
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

            // Find stylist
            HairStylist stylist = _firestoreProvider.Get<HairStylist>(appointmentConfirmationItem.SelectedStylist, _cancellationToken).Result;
            DocumentReference stylistRef = _firestoreProvider.ConvertIdToReference<HairStylist>(appointmentConfirmationItem.SelectedStylist);


            // Make sure stylist doesn't already have an appointment Scheduled
            DateTime appointmentToSchedule = new DateTime(appointmentConfirmationItem.DateOfAppointment.Year, appointmentConfirmationItem.DateOfAppointment.Month,
                    appointmentConfirmationItem.DateOfAppointment.Day, appointmentConfirmationItem.TimeOfAppointment.Hour, appointmentConfirmationItem.TimeOfAppointment.Minute,
                    appointmentConfirmationItem.TimeOfAppointment.Second);
            List<Appointment> stylistsAppointments = _firestoreProvider.WhereEqualTo<Appointment>("HairStylist", stylistRef, _cancellationToken).Result.ToList();
            bool alreadyBooked = stylistsAppointments.Any(x => {
                var existingApt = x.StartTimeOfAppointment.ToDateTime().ToLocalTime();
                return existingApt <= appointmentToSchedule && existingApt.AddHours(2) > appointmentToSchedule;
            });

            if (alreadyBooked)
            {
                // We want to show the user available times for the stylist!
                _logger.LogInformation(String.Format("Stylist {0} {1} is already booked! Alerting the user!.", stylist.FirstName, stylist.LastName));
                result.AppointmentConfirmationResultStatus = AppointmentConfirmationResultStatus.StylistAlreadyBooked;
                return result;
            }



            // Successful validation, do the handling

            // Get the client information for a returning client based on the phone number
            // If it is supposed to be a new client, the phone number should be unique to the new client
            // Else this is a new client that will be created later after confirmation!
            Client client;
            bool clientFound = false;

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

            // Save information to return object
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
