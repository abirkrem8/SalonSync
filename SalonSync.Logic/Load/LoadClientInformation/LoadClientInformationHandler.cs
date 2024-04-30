using AutoMapper;
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

namespace SalonSync.Logic.Load.LoadClientInformation
{
    public class LoadClientInformationHandler
    {
        private ILogger<LoadClientInformationHandler> _logger;
        private IMapper _mapper;
        private FirestoreProvider _firestoreProvider;
        private CancellationToken _cancellationToken;

        public LoadClientInformationHandler(ILogger<LoadClientInformationHandler> logger, IMapper mapper, FirestoreProvider firestoreProvider)
        {
            _logger = logger;
            _mapper = mapper;
            _firestoreProvider = firestoreProvider;
            _cancellationToken = new CancellationTokenSource().Token;
        }

        public LoadClientInformationResult Handle(LoadClientInformationItem loadClientInformationItem)
        {
            LoadClientInformationResult result = new LoadClientInformationResult();

            LoadClientInformationValidator validator = new LoadClientInformationValidator();
            var validationResult = validator.Validate(loadClientInformationItem);

            if (!validationResult.IsValid)
            {
                // There was an error in validation, quit now
                string error = string.Format("Validation Error: {0}", validationResult.Errors.FirstOrDefault());
                _logger.LogError(error);
                result.LoadClientInformationResultStatus = LoadClientInformationResultStatus.ValidationError;
                result.LoadClientInformationResultErrors.Add(new Error() { Message = error });
                return result;
            }

            // Successful validation, do the handling
            try
            {
                _logger.LogInformation("Loading Client Information...");
                Client client = _firestoreProvider.Get<Client>(loadClientInformationItem.ClientId, _cancellationToken).Result;
                DocumentReference clientRef = _firestoreProvider.ConvertIdToReference<Client>(loadClientInformationItem.ClientId);

                var appointmentsFromDB = _firestoreProvider.WhereEqualTo<Appointment>("Client", clientRef, _cancellationToken).Result.ToList();
                List<LoadClientInformationResultAppointment> pastAppointments = new List<LoadClientInformationResultAppointment>();
                List<LoadClientInformationResultAppointment> upcomingAppointments = new List<LoadClientInformationResultAppointment>();

                // Separate appointments into the past and future
                appointmentsFromDB.ForEach(appointmentFromDB =>
                {
                    var apt = _mapper.Map<LoadClientInformationResultAppointment>(appointmentFromDB);

                    if (apt.AppointmentStartTime < DateTime.Now)
                    {
                        pastAppointments.Add(apt);
                    }
                    else
                    {
                        upcomingAppointments.Add(apt);
                    }
                });

                // Save all info needed for UI
                result.ClientId = loadClientInformationItem.ClientId;
                result.ClientFullName = string.Concat(client.FirstName, " ", client.LastName);
                result.ClientPhoneNumber = client.PhoneNumber;
                result.ClientHairTexture = client.HairTexture;
                result.ClientHairLength = client.HairLength;
                result.PastAppointmentList = pastAppointments.OrderByDescending(x => x.AppointmentStartTime).ToList();
                result.UpcomingAppointmentList = upcomingAppointments.OrderByDescending(x => x.AppointmentStartTime).ToList();
                result.LoadClientInformationResultStatus = LoadClientInformationResultStatus.Success;

                _logger.LogInformation(string.Format("Successfully loaded Client information for {0}", result.ClientFullName));
                return result;
            }
            catch (Exception ex)
            {
                // Exceptions with the Firebase DB
                // log the error
                string error = string.Format("Database Error! {0}", ex.Message);
                _logger.LogError(error);
                result.LoadClientInformationResultStatus = LoadClientInformationResultStatus.DatabaseError;
                result.LoadClientInformationResultErrors.Add(new Error { Message = error });
                return result;
            }
        }
    }
}
