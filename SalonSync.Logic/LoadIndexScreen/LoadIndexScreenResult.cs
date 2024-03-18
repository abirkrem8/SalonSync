using SalonSync.Models.Entities;
using SalonSync.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.LoadIndexScreen
{
    public class LoadIndexScreenResult
    {
        public string CalendarEvents { get; set; }
        public List<HairStylist> HairStylists { get; set; }

        public LoadIndexScreenResultStatus LoadIndexScreenResultStatus { get; set; }
        public List<Error> LoadIndexScreenResultErrors { get; set; } = new List<Error>();
    }



    public enum LoadIndexScreenResultStatus
    {
        Success,
        ValidationError
    }
}

