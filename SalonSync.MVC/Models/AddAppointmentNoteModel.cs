namespace SalonSync.MVC.Models
{
    public class AddAppointmentNoteModel
    {
        public string ClientId { get; set; }
        public string AppointmentId { get; set; }
        public string NoteText { get; set; }
    }
}
