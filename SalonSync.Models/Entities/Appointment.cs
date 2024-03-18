using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Models.Entities
{
    [FirestoreData]
    public class Appointment : IFirebaseEntity
    {
        public Appointment() { }
        public Appointment(DocumentReference stylist, DocumentReference client, string clientFirstName, string clientLastName, DateTime scheduledTime)
        {
            Id = Guid.NewGuid().ToString();
            CreationTimestamp = Timestamp.GetCurrentTimestamp();
            HairStylist = stylist;
            Client = client;
            var utc = scheduledTime.ToUniversalTime();
            DateTimeOfAppointment = Timestamp.FromDateTime(utc);
            ClientFullName = string.Concat(clientFirstName, " ", clientLastName);
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
        public Timestamp DateTimeOfAppointment { get; set; }




        // These three are simply for making displaying the event on the calendar easier
        [FirestoreProperty]
        public string ClientFullName { get; set; }


        // String ID connecting to the other Firestore Data Objects
        [FirestoreProperty]
        public DocumentReference HairStylist { get; set; }

        // String ID connecting to the other Firestore Data Objects
        [FirestoreProperty]
        public DocumentReference Client { get; set; }

        // String IDs connecting to the other Firestore Data Objects
        [FirestoreProperty]
        public List<DocumentReference> HairChanges { get; set; } = new List<DocumentReference>();
    }
}
