using HairApplication.Models.Entities;

namespace HairApplication.MVC.Models
{
    public class IndexViewModel
    {
        public string CalendarEvents { get; set; }
        public List<HairStylist> HairStylists { get; set; }
    }
}
