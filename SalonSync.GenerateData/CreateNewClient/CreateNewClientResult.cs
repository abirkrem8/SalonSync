using SalonSync.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairApplication.Logic.CreateNewClient
{
    public class CreateNewClientResult
    {
        public CreateNewClientResultStatus CreateNewClientResultStatus { get; set; }
        public List<Error> CreateNewClientResultErrors { get; set; }


    }



    public enum CreateNewClientResultStatus
    {
        Success,
        ValidationError
    }
}
