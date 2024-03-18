using SalonSync.Models.Entities;

namespace SalonSync.MVC.Models
{
    public class IndexViewModel
    {
        public string CalendarEvents { get; set; }
        public List<HairStylist> HairStylists { get; set; }
    }
}
