using SalonSync.Models.Entities;
using SalonSync.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.Load.LoadStylistInformation
{
    public class LoadStylistInformationResult
    {
        public HairStylist HairStylist { get; set; }
        public List<Client> Clients { get; set; }

        // One week margin, sort by closest to current time
        public List<LoadStylistInformationResultAppointment> PastAppointments { get; set; } = new List<LoadStylistInformationResultAppointment>();
        public List<LoadStylistInformationResultAppointment> UpcomingAppointments { get; set; } = new List<LoadStylistInformationResultAppointment>();


        public LoadStylistInformationResultStatus LoadStylistInformationResultStatus { get; set; }
        public List<Error> LoadStylistInformationResultErrors { get; set; }

    }

    public class LoadStylistInformationResultAppointment
    {
        public string ClientFullName { get; set; }
        public DateTime DateTimeOfAppointment { get; set; }
        public string ClientPhoneNumber { get; set; }

        public string ClientId { get; set; }

        public string AppointmentId { get;set; }
    }



    public enum LoadStylistInformationResultStatus
    {
        Success,
        ValidationError
    }


}
