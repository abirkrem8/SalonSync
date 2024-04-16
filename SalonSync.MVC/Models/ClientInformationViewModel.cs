namespace SalonSync.MVC.Models
{
    public class ClientInformationViewModel
    {
        public string ClientFullName { get; set; }
        public string ClientPhoneNumber { get; set; }
        public List<ClientInformationViewModelAppointment> AppointmentList { get; set; } = new List<ClientInformationViewModelAppointment>();
    }

    public class ClientInformationViewModelAppointment
    {
        public string AppointmentType { get; set; }
        public int AppointmentCost { get; set; }
        public string HairStylistFullName { get; set; }
        public DateTime AppointmentStartTime { get; set; }
        public List<string> AppointmentNotes { get; set; }

    }
}
