using SalonSync.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.Load.LoadClientList
{
    public class LoadClientListResult
    {
        public List<LoadClientListResultItem> ClientList { get; set; }
        public LoadClientListResultStatus LoadClientListResultStatus { get; set; }
        public List<Error> LoadClientListResultErrors { get; set; }


    }

    public class LoadClientListResultItem
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }

    public enum LoadClientListResultStatus
    {
        Success,
        ValidationError
    }
}
