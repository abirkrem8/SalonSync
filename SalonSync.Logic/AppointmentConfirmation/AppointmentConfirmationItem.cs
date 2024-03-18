using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.AppointmentConfirmation
{
    public class AppointmentConfirmationItem
    {
        public string SelectedStylist { get; set; }
        public string ClientFirstName { get; set; }
        public string ClientLastName { get; set; }
        public string ClientPhoneNumber { get; set; }
        public bool IsNewClient { get; set; }
        public DateTime DateOfAppointment { get; set; }
        public DateTime TimeOfAppointment { get; set; }
    }
}
