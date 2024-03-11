using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairApplication.Models.Entities
{
    public class Appointment : EntityBase
    {
        public DateTime DateTime { get; set; }
        public HairStylist HairStylist { get; set; }
        public Client Client { get; set; }
        public List<HairChange> HairChanges { get; set; }
    }
}
