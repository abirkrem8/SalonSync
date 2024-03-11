using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairApplication.Models.Entities
{
    public class HairStylist : EntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int YearsWorked { get; set; }
        public List<Appointment> Appointments { get; set; }
    }
}
