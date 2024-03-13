using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairApplication.Models.Entities
{
    [FirestoreData]
    public class Appointment : IFirebaseEntity
    {
        // In the form of a GUID, easily convertable to a string
        [FirestoreProperty]
        public DocumentReference Id { get; set; }

        // Format MM/dd/yyyy HH:MM:SS
        [FirestoreProperty]
        public DateTime CreationTimestamp { get; set; }

        // User Name
        [FirestoreProperty]
        public string CreatedByUserName { get; set; }


        [FirestoreProperty]
        public string DateTimeOfAppointment { get; set; }

        // String ID connecting to the other Firestore Data Objects
        [FirestoreProperty]
        public string HairStylist { get; set; }

        // String ID connecting to the other Firestore Data Objects
        [FirestoreProperty]
        public string Client { get; set; }

        // String IDs connecting to the other Firestore Data Objects
        [FirestoreProperty]
        public List<DocumentReference> HairChanges { get; set; }
    }
}
