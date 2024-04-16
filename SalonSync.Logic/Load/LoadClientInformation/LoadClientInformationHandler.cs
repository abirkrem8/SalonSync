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
                string error = "Validation Error";
                _logger.LogError(error);
                result.LoadClientInformationResultStatus = LoadClientInformationResultStatus.ValidationError;
                result.LoadClientInformationResultErrors.Add(new Error() { Message = error });
                return result;
            }

            // Successful validation, do the handling
            _logger.LogInformation("Loading Client information...");
            Client client = _firestoreProvider.Get<Client>(loadClientInformationItem.ClientId, _cancellationToken).Result;
            DocumentReference clientRef = _firestoreProvider.ConvertIdToReference<Client>(loadClientInformationItem.ClientId);

            var appointmentsFromDB = _firestoreProvider.WhereEqualTo<Appointment>("Client", clientRef, _cancellationToken).Result.ToList();
            List<LoadClientInformationResultAppointment> appointments = new List<LoadClientInformationResultAppointment>();

            appointmentsFromDB.ForEach(appointmentFromDB =>
            {
                var apt = _mapper.Map<LoadClientInformationResultAppointment>(appointmentFromDB);
                appointments.Add(apt);
            });

            result.ClientFullName = string.Concat(client.FirstName, " ", client.LastName);
            result.ClientPhoneNumber = client.PhoneNumber;
            result.AppointmentList = appointments.OrderByDescending(x => x.AppointmentStartTime).ToList();
            result.LoadClientInformationResultStatus = LoadClientInformationResultStatus.Success;

            return result;
        }
    }
}
