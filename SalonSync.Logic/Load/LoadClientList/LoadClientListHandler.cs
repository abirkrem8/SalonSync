using AutoMapper;
using Microsoft.Extensions.Logging;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using SalonSync.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.Load.LoadClientList
{
    public class LoadClientListHandler
    {
        private ILogger<LoadClientListHandler> _logger;
        private FirestoreProvider _firestoreProvider;
        private CancellationToken _cancellationToken;
        private IMapper _mapper;

        public LoadClientListHandler(ILogger<LoadClientListHandler> logger, FirestoreProvider firestoreProvider,
            IMapper mapper)
        {
            _logger = logger;
            _firestoreProvider = firestoreProvider;
            _cancellationToken = new CancellationTokenSource().Token;
            _mapper = mapper;
        }

        public LoadClientListResult Handle(LoadClientListItem LoadClientListItem)
        {
            LoadClientListResult result = new LoadClientListResult();

            LoadClientListValidator validator = new LoadClientListValidator();
            var validationResult = validator.Validate(LoadClientListItem);

            if (!validationResult.IsValid)
            {
                // There was an error in validation, quit now
                // log the error
                string error = string.Format("Validation Error: {0}", validationResult.Errors.FirstOrDefault());
                _logger.LogError(error);
                result.LoadClientListResultStatus = LoadClientListResultStatus.ValidationError;
                result.LoadClientListResultErrors.Add(new Error() { Message = error });
                return result;
            }

            // Successful validation, do the handling
            try
            {
                // We want to grab all of the clients to display in a table on the UI
                var allClients = _firestoreProvider.GetAll<Client>(_cancellationToken).Result.ToList();

                result.ClientList = _mapper.Map<List<LoadClientListResultItem>>(allClients).OrderBy(x => x.FirstName).ToList();
                result.LoadClientListResultStatus = LoadClientListResultStatus.Success;
                return result;
            }
            catch (Exception ex)
            {
                // Exceptions with the Firebase DB
                // log the error
                string error = string.Format("Database Error! {0}", ex.Message);
                _logger.LogError(error);
                result.LoadClientListResultStatus = LoadClientListResultStatus.DatabaseError;
                result.LoadClientListResultErrors.Add(new Error { Message = error });
                return result;
            }
        }
    }
}
