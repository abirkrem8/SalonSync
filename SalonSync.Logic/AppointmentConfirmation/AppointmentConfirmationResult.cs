using SalonSync.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.AppointmentConfirmation
{
    public class AppointmentConfirmationResult
    {
        public string HairStylistFirstName { get; set; }
        public string HairStylistLastName { get; set; }
        public string HairStylistId { get; set; }
        public string ClientFirstName { get; set; }
        public string ClientLastName { get; set; }
        public string ClientPhoneNumber { get; set; }
        public string ClientId { get; set; }
        public bool IsNewClient { get; set; }
        public bool ExistingClientFound { get; set; }
        public DateTime DateTimeOfAppointment { get; set; }

        public AppointmentConfirmationResultStatus AppointmentConfirmationResultStatus { get; set; }
        public List<Error> AppointmentConfirmationResultErrors { get; set; } = new List<Error>();
    }



    public enum AppointmentConfirmationResultStatus
    {
        Success,
        StylistAlreadyBooked,
        DatabaseError,
        ValidationError
    }
}

