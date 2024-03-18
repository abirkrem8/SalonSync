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

namespace SalonSync.GenerateData
{
    public interface IAppointmentScheduleService
    {
        int Run(int numberOfDaysToSchedule);
    }

    public class AppointmentScheduleService : IAppointmentScheduleService
    {
        private AppointmentScheduleHandler _appointmentScheduleHandler;
        private FirestoreProvider _firestoreProvider;
        private ILogger<AppointmentScheduleService> _logger;
        private Random _random;
        private CancellationToken _cancellationToken;
        private const int MAX_APPOINTMENTS = 10;

        public AppointmentScheduleService(ILogger<AppointmentScheduleService> logger,
            AppointmentScheduleHandler appointmentScheduleHandler, FirestoreProvider firestoreProvider)
        {
            _appointmentScheduleHandler = appointmentScheduleHandler;
            _firestoreProvider = firestoreProvider;
            _logger = logger;
            _random = new Random();
            _cancellationToken = new CancellationTokenSource().Token;
        }


        public int Run(int numberOfDaysToSchedule)
        {
            _logger.LogInformation("In the Appointment Scheduling Service!");

            // First grab all of the clients
            var listOfClients = _firestoreProvider.GetAll<Client>(_cancellationToken).Result.ToList();
            bool scheduleAllClientsEachDay = false;
            if (listOfClients.Count < MAX_APPOINTMENTS)
            {
                _logger.LogInformation("Number of clients is {0}, scheduling all for appointments.", listOfClients.Count);
                scheduleAllClientsEachDay = true;
            } else
            {
                _logger.LogInformation("Number of clients is {0}, scheduling {1} appointments per day.", MAX_APPOINTMENTS);
            }


            // For each date, schedule {MAX_APPOINTMENTS} appointments
            for (int daysAhead = 1; daysAhead <= numberOfDaysToSchedule; daysAhead++)
            {
                DateTime scheduleDate = DateTime.Now.AddDays(daysAhead);

                // Randomly select clients to had scheduled appointments for this day
                List<int> clientIndexes = new List<int>();
                if (!scheduleAllClientsEachDay)
                {
                    int clientNum;
                    for (int numAppointments = 0; numAppointments < MAX_APPOINTMENTS; numAppointments++)
                    {
                        do
                        {
                            clientNum = _random.Next(0, listOfClients.Count() - 1);
                        } while (clientIndexes.Contains(clientNum));
                        clientIndexes.Add(clientNum);
                    }
                }
                else
                {
                    // if there is a small number of clients, we want to fill our books so we will schedule
                    // them each once a day!
                    clientIndexes = Enumerable.Range(0, listOfClients.Count()).ToList();
                }

                // For each client we will be scheduling for, generate a random appointment time
                foreach (int indexOfClient in clientIndexes)
                {
                    Client client = listOfClients[indexOfClient];

                    DateTime dateTimeOfAppointment = new DateTime(scheduleDate.Year, scheduleDate.Month, scheduleDate.Day,
                        _random.Next(8, 16), _random.Next(0, 3) * 15, 0);

                    var scheduleItem = new AppointmentScheduleItem()
                    {
                        IsNewClient = false,
                        FirstName = client.FirstName,
                        LastName = client.LastName,
                        DateTimeOfApppointment = dateTimeOfAppointment,
                        HairStylist = client.HairStylist.Id,
                        PhoneNumber = client.PhoneNumber

                    };
                    _logger.LogInformation(String.Format("Scheduling {0} {1} for an appointment at {2}!"), scheduleItem.FirstName, scheduleItem.LastName, scheduleItem.DateTimeOfApppointment.ToString("MM/dd/yyyy hh:mm tt"));
                    var result = _appointmentScheduleHandler.Handle(scheduleItem);
                    if (result.AppointmentScheduleResultStatus != AppointmentScheduleResultStatus.Success)
                    {
                        // log error!
                        _logger.LogError("There was an error!");
                    }
                }
            }

            return 0;
        }

    }
}
