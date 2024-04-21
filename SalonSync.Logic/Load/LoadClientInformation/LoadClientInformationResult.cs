using SalonSync.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.Load.LoadClientInformation
{
    public class LoadClientInformationResult
    {
        public string ClientFullName { get; set; }
        public string ClientPhoneNumber { get; set; }
        public List<LoadClientInformationResultAppointment> UpcomingAppointmentList { get; set; } = new List<LoadClientInformationResultAppointment>();
        public List<LoadClientInformationResultAppointment> PastAppointmentList { get; set; } = new List<LoadClientInformationResultAppointment>();
        public LoadClientInformationResultStatus LoadClientInformationResultStatus { get; set; }
        public List<Error> LoadClientInformationResultErrors { get; set; }


    }

    public class LoadClientInformationResultAppointment
    {
        public string AppointmentType { get; set; }
        public int AppointmentCost { get; set; }
        public string HairStylistFullName { get; set; }
        public DateTime AppointmentStartTime { get; set; }
        public List<string> AppointmentNotes { get; set; }
    }



    public enum LoadClientInformationResultStatus
    {
        Success,
        ValidationError
    }
}
