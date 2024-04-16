using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalonSync.Models.Enums;

namespace SalonSync.Models.Entities
{
    [FirestoreData]
    public class Client : IFirebaseEntity
    {
        public Client() { }

        public Client(string firstName, string lastName, string phoneNumber, Enums.HairTexture hairTexture, Enums.HairLength hairLength)
        {
            Id = Guid.NewGuid().ToString();
            CreationTimestamp = Timestamp.GetCurrentTimestamp();
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            HairTexture = hairTexture.ToString();
            HairLength = hairLength.ToString();
        }

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
        public string PhoneNumber { get; set; }

        [FirestoreProperty]
        public string HairTexture { get; set; }

        [FirestoreProperty]
        public string HairLength { get; set; }

    }
}
