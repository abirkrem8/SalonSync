using SalonSync.Logic.AppointmentSchedule;
using Microsoft.Extensions.Logging;
using SalonSync.Logic.AppointmentSchedule;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.DeleteAppointments
{
    public interface IAppointmentDeletionService
    {
        int Run(int numberOfDaysToLookBack, bool deleteAllApppointments);
    }

    public class AppointmentDeletionService : IAppointmentDeletionService
    {
        private FirestoreProvider _firestoreProvider;
        private ILogger<AppointmentDeletionService> _logger;
        private Random _random;
        private CancellationToken _cancellationToken;
        private const int MAX_APPOINTMENTS = 10;

        public AppointmentDeletionService(ILogger<AppointmentDeletionService> logger,
            FirestoreProvider firestoreProvider)
        {
            _firestoreProvider = firestoreProvider;
            _logger = logger;
            _random = new Random();
            _cancellationToken = new CancellationTokenSource().Token;
        }


        public int Run(int numberOfDaysToLookBack, bool deleteAllApppointments = false)
        {
            _logger.LogInformation("In the Appointment Deletion Service!");

            var allAppointments = _firestoreProvider.GetAll<Appointment>(_cancellationToken).Result.ToList();
            List<Appointment> appointmentsToDelete = new List<Appointment>();

            if (deleteAllApppointments)
            {
                appointmentsToDelete = allAppointments;
            }
            else
            {
                appointmentsToDelete = allAppointments.Where(a =>
                    a.StartTimeOfAppointment.ToDateTime().ToLocalTime() < DateTime.Now
                    && a.StartTimeOfAppointment.ToDateTime().ToLocalTime() > DateTime.Now.AddDays(numberOfDaysToLookBack * -1).Date).ToList();
            }

            appointmentsToDelete.ForEach(a =>
            {
                DeleteAppointmentFromDB(a);
            });

            return 0;
        }

        private int DeleteAppointmentFromDB(Appointment apt)
        {
            // Need to delete from Stylist
            var stylist = _firestoreProvider.Get<HairStylist>(apt.HairStylist.Id, _cancellationToken).Result;
            int numRemoved = stylist.Appointments.RemoveAll(a => a.Id == apt.Id);
            _firestoreProvider.AddOrUpdate(stylist, _cancellationToken).Wait();

            if (numRemoved == 0)
            {
                _logger.LogInformation(String.Format("Appointment id: {0} not found for stylist {1}.", apt.Id, stylist.Id));
            }

            // Need to delete from Client
            var client = _firestoreProvider.Get<Client>(apt.Client.Id, _cancellationToken).Result;
            numRemoved = client.Appointments.RemoveAll(a => a.Id == apt.Id);
            _firestoreProvider.AddOrUpdate(client, _cancellationToken).Wait();

            if (numRemoved == 0)
            {
                _logger.LogInformation(String.Format("Appointment id: {0} not found for client {1}.", apt.Id, client.Id));
            }

            // Delete from Appointment Table
            var aptRef = _firestoreProvider.ConvertIdToReference<Appointment>(apt.Id);
            aptRef.DeleteAsync().Wait();
            _logger.LogInformation(String.Format("Removed appointment {0} from the SalonSync Database", apt.Id));
            return 0;
        }

    }
}
