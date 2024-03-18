using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairApplication.Logic.LoadStylistInformation
{
    public class LoadStylistInformationHandler
    {
        private ILogger<LoadStylistInformationHandler> _logger;

        public LoadStylistInformationHandler(ILogger<LoadStylistInformationHandler> logger)
        {
            _logger = logger;
        }

        public LoadStylistInformationResult Handle(LoadStylistInformationItem LoadStylistInformationItem)
        {
            LoadStylistInformationResult result = new LoadStylistInformationResult();

            LoadStylistInformationValidator validator = new LoadStylistInformationValidator();
            var validationResult = validator.Validate(LoadStylistInformationItem);

            if (!validationResult.IsValid)
            {
                // There was an error in validation, quit now
                // log the error
                return result;
            }

            // Successful validation, do the handling

            return result;
        }
    }
}
