﻿using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairApplication.Models.Entities
{
    [FirestoreData]
    public class HairStylist : IFirebaseEntity
    {
        // In the form of a GUID, easily convertable to a string
        //[FirestoreProperty]
        //public DocumentReference Id { get; set; }

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

        // String ID connecting to the other Firestore Data Objects
        [FirestoreProperty]
        public List<DocumentReference> Appointments { get; set; } = new List<DocumentReference>();
    }
}
