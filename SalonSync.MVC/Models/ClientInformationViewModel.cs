namespace SalonSync.MVC.Models
{
    public class ClientInformationViewModel
    {
        public string ClientId { get; set; } 
        public string ClientFullName { get; set; }
        public string ClientPhoneNumber { get; set; }
        public string ClientHairTexture { get; set; }
        public string ClientHairLength { get; set; }
        public List<ClientInformationViewModelAppointment> PastAppointmentList { get; set; } = new List<ClientInformationViewModelAppointment>();
        public List<ClientInformationViewModelAppointment> UpcomingAppointmentList { get; set; } = new List<ClientInformationViewModelAppointment>();
    }

    public class ClientInformationViewModelAppointment
    {
        public string AppointmentId { get; set; }
        public string AppointmentType { get; set; }
        public int AppointmentCost { get; set; }
        public string HairStylistFullName { get; set; }
        public DateTime AppointmentStartTime { get; set; }
        public List<string> AppointmentNotes { get; set; }

    }
}
