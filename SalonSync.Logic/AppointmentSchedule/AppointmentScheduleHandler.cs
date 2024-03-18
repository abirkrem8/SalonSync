﻿using Google.Cloud.Firestore;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.AppointmentSchedule
{
    public class AppointmentScheduleHandler
    {
        private FirestoreProvider _firestoreProvider;
        private CancellationToken _cancellationToken;

        public AppointmentScheduleHandler(FirestoreProvider firestoreProvider)
        {
            _firestoreProvider = firestoreProvider;
            _cancellationToken = new CancellationTokenSource().Token;
        }

        public AppointmentScheduleResult Handle(AppointmentScheduleItem appointmentScheduleItem)
        {
            AppointmentScheduleResult result = new AppointmentScheduleResult();

            AppointmentScheduleValidator validator = new AppointmentScheduleValidator();
            var validationResult = validator.Validate(appointmentScheduleItem);

            if (!validationResult.IsValid)
            {
                // There was an error in validation, quit now
                // log the error
                return result;
            }

            // Successful validation, do the handling
            HairStylist hairStylist;
            DocumentReference stylistReference;
            Client client;
            DocumentReference clientReference;
            Appointment appointment;
            DocumentReference appointmentReference;


            // Grab the stylist reference
            hairStylist = _firestoreProvider.Get<HairStylist>(appointmentScheduleItem.HairStylist, _cancellationToken).Result;
            stylistReference = _firestoreProvider.ConvertIdToReference<HairStylist>(hairStylist.Id);

            // New client? Add to DB and grab Reference
            if (appointmentScheduleItem.IsNewClient)
            {
                client = new Client(appointmentScheduleItem.FirstName, appointmentScheduleItem.LastName,
                    appointmentScheduleItem.PhoneNumber, stylistReference);

                clientReference = _firestoreProvider.AddOrUpdate(client, _cancellationToken).Result;
            }
            else
            {
                // grab all clients and find the ONE with the same phone number
                var clients = _firestoreProvider.GetAll<Client>(_cancellationToken).Result.ToList();

                var matchingClients = clients.Where(x => x.PhoneNumber.Equals(appointmentScheduleItem.PhoneNumber)).ToList();
                if (matchingClients.Count() != 1)
                {
                    // throw error!
                    // log error and send it back to view model
                    return result;
                }
                client = matchingClients[0];
                clientReference = _firestoreProvider.ConvertIdToReference<Client>(client.Id);
            }

            // Create Appointment object and add to DB, grab ID
            appointment = new Appointment(stylistReference, clientReference, client.FirstName, client.LastName, appointmentScheduleItem.DateTimeOfApppointment);
            if (!AddAppointmentToDatabase(client, hairStylist, appointment))
            {
                result.AppointmentScheduleResultStatus = AppointmentScheduleResultStatus.DatabaseError;
                // log error
                return result;
            }
            result.AppointmentScheduleResultStatus = AppointmentScheduleResultStatus.Success;

            return result;
        }

        private bool AddAppointmentToDatabase(Client client, HairStylist hairStylist, Appointment appointment)
        {
            // Add Appointment object to DB, grab ID
            var appointmentReference = _firestoreProvider.AddOrUpdate(appointment, _cancellationToken).Result;

            // Update Client DB with appointment
            if (client.Appointments == null)
            {
                client.Appointments = new List<DocumentReference>();
            }
            client.Appointments.Add(appointmentReference);
            var clientUpdateResult = _firestoreProvider.AddOrUpdate(client, _cancellationToken).Result;

            // Update Stylist DB with appointment
            if (hairStylist.Appointments == null)
            {
                hairStylist.Appointments = new List<DocumentReference>();
            }
            hairStylist.Appointments.Add(appointmentReference);
            var stylistUpdateResult = _firestoreProvider.AddOrUpdate(hairStylist, _cancellationToken).Result;

            // do something with update results to make sure they returned successfully
            return true;
        }
    }
}