using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using RandomNameGeneratorLibrary;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using SalonSync.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairApplication.Logic.CreateNewClient
{
    public class CreateNewClientHandler
    {
        private ILogger<CreateNewClientHandler> _logger;
        private PersonNameGenerator _personGenerator;
        private Random _random;
        private FirestoreProvider _firestoreProvider;
        private CancellationToken _cancellationToken;

        public CreateNewClientHandler(ILogger<CreateNewClientHandler> logger, FirestoreProvider firestoreProvider)
        {
            _logger = logger;
            _personGenerator = new PersonNameGenerator();
            _random = new Random();
            _firestoreProvider = firestoreProvider;
            _cancellationToken = new CancellationTokenSource().Token;
        }

        public CreateNewClientResult Handle(CreateNewClientItem CreateNewClientItem)
        {
            CreateNewClientResult result = new CreateNewClientResult();

            CreateNewClientValidator validator = new CreateNewClientValidator();
            var validationResult = validator.Validate(CreateNewClientItem);

            if (!validationResult.IsValid)
            {
                // There was an error in validation, quit now
                // log the error
                return result;
            }


            // Successful validation, do the handling

            for (int i = 0; i < CreateNewClientItem.NumberOfNewClientsToCreate; i++)
            {
                try
                {
                    var client = new Client()
                    {
                        FirstName = _random.GenerateRandomFirstName(),
                        LastName = _random.GenerateRandomLastName(),
                        PhoneNumber = GenerateRandomPhoneNumber(),
                        HairLength = ((HairLength)_random.Next(1, 3)).GetDisplayName(),
                        HairTexture = ((HairTexture)_random.Next(1, 3)).GetDisplayName(),
                        CreationTimestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                        Id = Guid.NewGuid().ToString()
                    };
                    _logger.LogInformation(string.Format("Creating new client {0} {1}.", client.FirstName, client.LastName));
                    _firestoreProvider.AddOrUpdate<Client>(client, _cancellationToken).Wait();
                } catch (Exception ex)
                {
                    _logger.LogError("Oh no! Error");
                }
            }

            result.CreateNewClientResultStatus = CreateNewClientResultStatus.Success;
            return result;
        }


        private string GenerateRandomPhoneNumber()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(_random.Next(100000, 999999));
            builder.Append(_random.Next(1000, 9999));
            return builder.ToString();
        }
    }
}
