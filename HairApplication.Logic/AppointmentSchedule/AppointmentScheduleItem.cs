using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairApplication.Logic.AppointmentSchedule
{
    public class AppointmentScheduleItem
    {
        // if true, add client to DB
        public bool NewClient { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateTimeOfApppointment { get; set; }
        public string HairStylist { get; set; }
        
    }
}
