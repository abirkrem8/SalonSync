using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairApplication.Logic.LoadStylistInformation
{
    public class LoadStylistInformationResult
    {
        public LoadStylistInformationResultStatus LoadStylistInformationResultStatus { get; set; }
        public List<Error> LoadStylistInformationResultErrors { get; set; }


    }



    public enum LoadStylistInformationResultStatus
    {
        Success,
        ValidationError
    }
}
