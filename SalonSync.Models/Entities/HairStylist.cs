using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Models.Entities
{
    [FirestoreData]
    public class HairStylist : IFirebaseEntity
    {
        // In the form of a GUID, easily convertable to a string

        [FirestoreProperty]
        public string Id { get; set; }

        // Format MM/dd/yyyy HH:MM:SS
        [FirestoreProperty]
        public Timestamp CreationTimestamp { get; set; }

        [FirestoreProperty]
        public string FirstName { get; set; }

        [FirestoreProperty]
        public string LastName { get; set; }

        [FirestoreProperty]
        public string HexColor { get; set; }

        [FirestoreProperty]
        public string ProfileImageURL { get; set; }

        // String ID connecting to the other Firestore Data Objects
        [FirestoreProperty]
        public List<DocumentReference> Appointments { get; set; } = new List<DocumentReference>();
    }
}
