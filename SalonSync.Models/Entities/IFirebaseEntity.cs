using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Models.Entities
{
    public interface IFirebaseEntity
    {
        public string Id { get; set; }
        public Timestamp CreationTimestamp { get; set; }
    }
}
