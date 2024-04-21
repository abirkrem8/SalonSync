using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.Load.LoadStylistInformation
{
    public class LoadStylistInformationHandler
    {
        private ILogger<LoadStylistInformationHandler> _logger;
        private FirestoreProvider _firestoreProvider;
        private CancellationToken _cancellationToken;

        public LoadStylistInformationHandler(ILogger<LoadStylistInformationHandler> logger, FirestoreProvider firestoreProvider)
        {
            _logger = logger;
            _firestoreProvider = firestoreProvider;
            _cancellationToken = new CancellationTokenSource().Token;
        }

        public LoadStylistInformationResult Handle(LoadStylistInformationItem loadStylistInformationItem)
        {
            LoadStylistInformationResult result = new LoadStylistInformationResult();

            LoadStylistInformationValidator validator = new LoadStylistInformationValidator();
            var validationResult = validator.Validate(loadStylistInformationItem);

            if (!validationResult.IsValid)
            {
                // There was an error in validation, quit now
                // log the error
                return result;
            }

            // Successful validation, do the handling
            // Grab the Stylist
            var stylist = _firestoreProvider.Get<HairStylist>(loadStylistInformationItem.HairStylistId, _cancellationToken).Result;
            var stylistReference = _firestoreProvider.ConvertIdToReference<HairStylist>(loadStylistInformationItem.HairStylistId);
            
            // Grab the clients for stylist while loading appointments
            var clientIds = new List<string>();

            // Grab last week's appointments and upcoming appointments
            var allAppointments = _firestoreProvider.WhereEqualTo<Appointment>("HairStylist", stylistReference, _cancellationToken).Result.ToList();
            var pastAppointments = new List<LoadStylistInformationResultAppointment>();
            var upcomingAppointments = new List<LoadStylistInformationResultAppointment>();
            allAppointments.ForEach(appointment =>
            {
                var appointmentTime = appointment.StartTimeOfAppointment.ToDateTime().ToLocalTime();

                var apt = new LoadStylistInformationResultAppointment()
                {
                    ClientFullName = appointment.ClientFullName,
                    AppointmentId = appointment.Id,
                    ClientPhoneNumber = appointment.ClientPhoneNumber,
                    DateTimeOfAppointment = appointmentTime,
                    ClientId = appointment.Client.Id
                };

                if (appointmentTime < DateTime.Now && appointmentTime.Date > DateTime.Now.AddDays(-7))
                {
                    // appointment has passed in the last 7 days
                    pastAppointments.Add(apt);
                } else if (appointmentTime > DateTime.Now && appointmentTime.Date < DateTime.Now.AddDays(7))
                {
                    // appointment is in the next 7 days
                    upcomingAppointments.Add(apt);
                }
                clientIds.Add(appointment.Client.Id);
            });

            result.HairStylist = stylist;
            result.Clients = FindClients(clientIds.Distinct().ToList());

            result.PastAppointments = pastAppointments.OrderByDescending(x => x.DateTimeOfAppointment).ToList();
            result.UpcomingAppointments= upcomingAppointments.OrderBy(x => x.DateTimeOfAppointment).ToList();
            result.LoadStylistInformationResultStatus = LoadStylistInformationResultStatus.Success;

            return result;
        }

        private List<Client> FindClients(List<string> clientIds)
        {
            var clients = new List<Client>();

            foreach(var clientId in clientIds)
            {
                clients.Add(_firestoreProvider.Get<Client>(clientId,_cancellationToken).Result);
            }
            return clients;
        }
    }
}
