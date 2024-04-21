using SalonSync.Models.Entities;
using SalonSync.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace SalonSync.MVC.Models
{
    public class AppointmentScheduleViewModel
    {
        public Dictionary<string, List<DateTime>> AvailableAppointmentsForEachStylist { get; set; }

        public List<HairStylist> HairStylists { get; set; } = new List<HairStylist>();
        [Display(Name = "Hair Stylist")]
        public string SelectedStylist { get; set; }

        [Display(Name = "New Client First Name")]
        public string ClientFirstName { get; set; }

        [Display(Name = "New Client Last Name")]
        public string ClientLastName { get; set; }

        [Display(Name = "Client Phone Number")]
        public string ClientPhoneNumber { get; set; }

        [Display(Name = "New Client?")]
        public bool IsNewClient { get; set; }

        [Display(Name = "Date of Appointment")]
        public DateTime DateOfAppointment { get; set; }

        [Display(Name = "Type of Appointment")]
        public AppointmentType AppointmentType { get; set; }

        [Display(Name = "New Client Hair Texture")]
        public HairTexture ClientHairTexture { get; set; }

        [Display(Name = "New Client Hair Length")]
        public HairLength ClientHairLength { get; set; }

        [Display(Name = "Time of Appointment")]
        public string TimeOfAppointment { get; set; }
    }
}
