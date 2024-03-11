using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairApplication.Models.Entities
{
    public class Client : EntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public HairStylist HairStylist { get; set; }
        public List<Appointment> Appointments { get; set; }

    }
}
