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
        int Run(CommandLineOptions options);
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


        public int Run(CommandLineOptions options)
        {
            _logger.LogInformation("In the Appointment Deletion Service!");

            var allAppointments = _firestoreProvider.GetAll<Appointment>(_cancellationToken).Result.ToList();
            List<Appointment> appointmentsToDelete = new List<Appointment>();

            if (options.DeleteAllAppointments)
            {
                appointmentsToDelete = allAppointments;
            }
            else
            {
                // options.ScheduleHistorically will not include scheduling for today
                // future scheduling will include today
                DateTime startDate = options.DeleteFutureAppointments ? DateTime.Now.Date : DateTime.Now.AddDays(options.NumberOfDaysToDelete * -1).Date;
                DateTime endDate = options.DeleteFutureAppointments ? DateTime.Now.AddDays(options.NumberOfDaysToDelete).Date : DateTime.Now.AddDays(-1).Date;

                appointmentsToDelete = allAppointments.Where(a =>
                    a.StartTimeOfAppointment.ToDateTime().ToLocalTime().Date <= endDate
                    && a.StartTimeOfAppointment.ToDateTime().ToLocalTime().Date >= startDate).ToList();
            }

            appointmentsToDelete.ForEach(a =>
            {
                DeleteAppointmentFromDB(a);
            });

            return 0;
        }

        private void DeleteAppointmentFromDB(Appointment apt)
        {
            // Delete from Appointment Table
            try
            {
                var aptRef = _firestoreProvider.ConvertIdToReference<Appointment>(apt.Id);
                aptRef.DeleteAsync().Wait();
                _logger.LogInformation(String.Format("Removed appointment {0} from the SalonSync Database", apt.Id));

            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Error while deleting appointment {0} from the database: {1}", apt.Id, ex.Message));

            }
        }

    }
}
