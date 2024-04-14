using SalonSync.Models.Entities;
using SalonSync.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.Load.LoadAppointmentScheduleForm
{
    public class LoadAppointmentScheduleFormResult
    {
        public Dictionary<string, List<DateTime>> AvailableAppointmentsForEachStylist { get; set; }
        public List<HairStylist> HairStylists { get; set; } = new List<HairStylist>();
        public LoadAppointmentScheduleFormResultStatus LoadAppointmentScheduleFormResultStatus { get; set; }
        public List<Error> LoadAppointmentScheduleFormResultErrors { get; set; }


    }

    public enum LoadAppointmentScheduleFormResultStatus
    {
        Success,
        ValidationError
    }
}
