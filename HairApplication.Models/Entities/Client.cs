using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairApplication.Models.Entities
{
    [FirestoreData]
    public class Client : IFirebaseEntity
    {
        public Client() { }

        public Client(string firstName, string lastName, string phoneNumber, DocumentReference stylist)
        {
            Id = Guid.NewGuid().ToString();
            CreationTimestamp = Timestamp.GetCurrentTimestamp();
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            HairStylist = stylist;
        }

        // In the form of a GUID, easily convertable to a string
        [FirestoreProperty]
        public string Id { get; set; }

        // Format MM/dd/yyyy HH:MM:SS
        [FirestoreProperty]
        public Timestamp CreationTimestamp { get; set; }

        // User Name
        [FirestoreProperty]
        public string CreatedByUserName { get; set; }

        [FirestoreProperty]
        public string FirstName { get; set; }

        [FirestoreProperty]
        public string LastName { get; set; }

        [FirestoreProperty]
        public string PhoneNumber { get; set; }

        // String ID connecting to the other Firestore Data Objects
        [FirestoreProperty]
        public DocumentReference HairStylist { get; set; }

        // String ID connecting to the other Firestore Data Objects
        [FirestoreProperty]
        public List<DocumentReference> Appointments { get; set; } = new List<DocumentReference>();

    }
}
