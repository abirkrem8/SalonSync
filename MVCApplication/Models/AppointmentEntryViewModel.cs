using HairApplication.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace HairApplication.MVC.Models
{
    public class AppointmentEntryViewModel
    {
        public List<HairStylist> Stylists { get; set; }
        

        [Display(Name = "Client Name")]
        public string Client { get; set; }
    }
}
