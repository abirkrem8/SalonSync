using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Models.Entities
{
    [FirestoreData]
    public class HairChange : IFirebaseEntity
    {
        // In the form of a GUID, easily convertable to a string
        [FirestoreProperty]
        public string Id { get; set; }

        // Format MM/dd/yyyy HH:MM:SS
        [FirestoreProperty]
        public Timestamp CreationTimestamp { get; set; }

        // User Name
        [FirestoreProperty]
        public string CreatedByUserName { get; set; }

        // Maybe grab this from available brands of the salon
        [FirestoreProperty]
        public string HairDyeBrand { get; set; }

        // From color picker
        [FirestoreProperty]
        public string HairColorHex { get; set; }

        // How much dye was used
        [FirestoreProperty]
        public int VolumeInmL { get; set; }

        // String ID connecting to the other Firestore Data Objects
        [FirestoreProperty]
        public DocumentReference Appointment { get; set; }

    }
}
