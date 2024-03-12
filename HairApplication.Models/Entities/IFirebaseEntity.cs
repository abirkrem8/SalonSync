using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairApplication.Models.Entities
{
    public interface IFirebaseEntity
    {
        public string Id { get; set; }
        public DateTime CreationTimestamp { get; set; }
        public string CreatedByUserName { get; set; }
    }
}
