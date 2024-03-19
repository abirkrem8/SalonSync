using SalonSync.Models.Entities;

namespace SalonSync.MVC.Models
{
    public class StylistDetailViewModel
    {
        public HairStylist HairStylist { get; set; }
        public List<Client> Clients { get; set; }

        // One week margin, sort by closest to current time
        public List<StylistDetailViewModelAppointment> PastAppointments { get; set; }
        public List<StylistDetailViewModelAppointment> UpcomingAppointments { get; set; }

    }

    public class StylistDetailViewModelAppointment
    {
        public string ClientFullName { get; set; }
        public DateTime DateTimeOfAppointment { get; set; }
        public string ClientPhoneNumber { get; set; }
        public string AppointmentId { get; set; }
    }
}
