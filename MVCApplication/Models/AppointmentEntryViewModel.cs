using HairApplication.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace HairApplication.MVC.Models
{
    public class AppointmentEntryViewModel
    {
        public List<HairStylist> AvailableStylists { get; set; }

        [Display(Name = "Hair Stylist")]
        public string SelectedSytlist { get; set; }
        [Display(Name = "Client First Name")]
        public string ClientFirstName { get; set; }

        [Display(Name = "Client Last Name")]
        public string ClientLastName { get; set; }

        [Display(Name = "Client Phone Number")]
        public int ClientPhoneNumber { get; set; }

        [Display(Name = "New Client?")]
        public bool IsNewClient { get; set; }

        [Display(Name = "Date of Appointment")]
        public DateTime DateOfAppointment { get; set; }

        [Display(Name = "Date of Appointment")]
        public DateTime TimeOfAppointment { get; set; }
    }
}
