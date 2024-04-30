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
                string error = string.Format("Validation Error: {0}", validationResult.Errors.FirstOrDefault());
                _logger.LogError(error);
                result.LoadStylistInformationResultStatus = LoadStylistInformationResultStatus.ValidationError;
                result.LoadStylistInformationResultErrors.Add(new Error() { Message = error });
                return result;
            }

            // Successful validation, do the handling
            try
            {
                // Grab the Stylist
                var stylist = _firestoreProvider.Get<HairStylist>(loadStylistInformationItem.HairStylistId, _cancellationToken).Result;
                var stylistReference = _firestoreProvider.ConvertIdToReference<HairStylist>(loadStylistInformationItem.HairStylistId);

                _logger.LogInformation(string.Format("Loading stylist information for {0} {1}.", stylist.FirstName, stylist.LastName));
                // Grab the clients for stylist while loading appointments
                var clientIds = new List<string>();

                // Grab last week's appointments and upcoming appointments for stylist
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
                    // Separate appointments by past and future appointments
                    if (appointmentTime < DateTime.Now && appointmentTime.Date > DateTime.Now.AddDays(-7))
                    {
                        // appointment has passed in the last 7 days
                        pastAppointments.Add(apt);
                    }
                    else if (appointmentTime > DateTime.Now && appointmentTime.Date < DateTime.Now.AddDays(7))
                    {
                        // appointment is in the next 7 days
                        upcomingAppointments.Add(apt);
                    }
                    clientIds.Add(appointment.Client.Id);
                });

                result.HairStylist = stylist;
                result.Clients = FindClients(clientIds.Distinct().ToList());

                result.PastAppointments = pastAppointments.OrderByDescending(x => x.DateTimeOfAppointment).ToList();
                result.UpcomingAppointments = upcomingAppointments.OrderBy(x => x.DateTimeOfAppointment).ToList();
                result.LoadStylistInformationResultStatus = LoadStylistInformationResultStatus.Success;
                _logger.LogInformation(string.Format("Successfully loading information for stylist {0} {1}", stylist.FirstName, stylist.LastName));
                return result;
            }
            catch (Exception ex)
            {
                // Exceptions with the Firebase DB
                // log the error
                string error = string.Format("Database Error while loading stylist! {0}", ex.Message);
                _logger.LogError(error);
                result.LoadStylistInformationResultStatus = LoadStylistInformationResultStatus.DatabaseError;
                result.LoadStylistInformationResultErrors.Add(new Error { Message = error });
                return result;
            }
        }

        private List<Client> FindClients(List<string> clientIds)
        {
            // Grab all clients by id
            var clients = new List<Client>();

            foreach (var clientId in clientIds)
            {
                clients.Add(_firestoreProvider.Get<Client>(clientId, _cancellationToken).Result);
            }
            return clients;
        }
    }
}
