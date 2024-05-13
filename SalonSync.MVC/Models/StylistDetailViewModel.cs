using SalonSync.Models.Entities;

namespace SalonSync.MVC.Models
{
    public class StylistDetailViewModel
    {
        public HairStylist HairStylist { get; set; } = new HairStylist();
        public List<Client> Clients { get; set; } = new List<Client>();

        // One week margin, sort by closest to current time
        public List<StylistDetailViewModelAppointment> PastAppointments { get; set; } = new List<StylistDetailViewModelAppointment>();
        public List<StylistDetailViewModelAppointment> UpcomingAppointments { get; set; } = new List<StylistDetailViewModelAppointment>();

    }

    public class StylistDetailViewModelAppointment
    {
        public string ClientFullName { get; set; }
        public string ClientId { get; set; }
        public DateTime DateTimeOfAppointment { get; set; }
        public string ClientPhoneNumber { get; set; }
        public string AppointmentId { get; set; }
    }
}
