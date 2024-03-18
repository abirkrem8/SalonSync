using SalonSync.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace SalonSync.MVC.Models
{
    public class AppointmentConfirmationViewModel
    {
        public string HairStylistFirstName { get; set; }
        public string HairStylistLastName { get; set; }
        public string HairStylistId { get; set; }
        public string ClientFirstName { get; set; }
        public string ClientLastName { get; set; }
        public string ClientPhoneNumber { get; set; }
        public string ClientId { get; set; }
        public bool IsNewClient { get; set; }
        public string DateTimeOfAppointment { get; set; }
    }
}
