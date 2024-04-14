﻿using Google.Cloud.Firestore;
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
        public Appointment(DocumentReference stylist, DocumentReference client, string clientFirstName, 
            string clientLastName, string clientPhoneNumber, DateTime scheduledDate, DateTime scheduledTime)
        {
            Id = Guid.NewGuid().ToString();
            CreationTimestamp = Timestamp.GetCurrentTimestamp();
            HairStylist = stylist;
            Client = client;
            ClientPhoneNumber = clientPhoneNumber;
            var aptDateTime = new DateTime(scheduledDate.Year, scheduledDate.Month, scheduledDate.Day, scheduledTime.Hour, scheduledTime.Minute, 0);
            StartTimeOfAppointment = Timestamp.FromDateTime(aptDateTime.ToUniversalTime());
            EndTimeOfAppointment = Timestamp.FromDateTime(aptDateTime.AddHours(2).ToUniversalTime());
            ClientFullName = string.Concat(clientFirstName, " ", clientLastName);
        }


        // In the form of a GUID, easily convertable to a string
        [FirestoreProperty]
        public string Id { get; set; }

        // Format MM/dd/yyyy HH:MM:SS
        [FirestoreProperty]
        public Timestamp CreationTimestamp { get; set; }


        [FirestoreProperty]
        public Timestamp StartTimeOfAppointment { get; set; }

        [FirestoreProperty]
        public Timestamp EndTimeOfAppointment { get; set; }


        // These three are simply for making displaying the event on the calendar easier
        [FirestoreProperty]
        public string ClientFullName { get; set; }

        // These three are simply for making displaying the event on the calendar easier
        [FirestoreProperty]
        public string ClientPhoneNumber { get; set; }


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
