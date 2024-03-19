using Google.Type;
using Microsoft.Extensions.Logging;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalonSync.Logic.Load.LoadIndexScreen
{
    public class LoadIndexScreenHandler
    {
        private FirestoreProvider _firestoreProvider;
        private ILogger<LoadIndexScreenHandler> _logger;
        private CancellationToken _cancellationToken;

        public LoadIndexScreenHandler(FirestoreProvider firestoreProvider, ILogger<LoadIndexScreenHandler> logger)
        {
            _firestoreProvider = firestoreProvider;
            _logger = logger;
            _cancellationToken = new CancellationTokenSource().Token;
        }

        public LoadIndexScreenResult Handle(LoadIndexScreenItem LoadIndexScreenItem)
        {
            LoadIndexScreenResult loadIndexScreenResult = new LoadIndexScreenResult();

            LoadIndexScreenValidator validator = new LoadIndexScreenValidator();
            var validationResult = validator.Validate(LoadIndexScreenItem);

            if (!validationResult.IsValid)
            {
                // There was an error in validation, quit now
                // log the error
                return loadIndexScreenResult;
            }

            // Successful validation, do the handling
            var appointments = _firestoreProvider.GetAll<Appointment>(_cancellationToken).Result.ToList();
            var stylists = _firestoreProvider.GetAll<HairStylist>(_cancellationToken).Result.ToList();
            var calendarEvents = new object[appointments.Count()];

            // Create all of the calendar events for the smart-scheduler
            for (int i = 0; i < appointments.Count; i++)
            {
                var aptStylist = stylists.FirstOrDefault(x => x.Id == appointments[i].HairStylist.Id);
                if (aptStylist != null)
                {
                    var appointmentStartTime = appointments[i].DateTimeOfAppointment.ToDateTime().ToLocalTime();
                    var description = string.Concat("This appointment is for ", appointments[i].ClientFullName,
                        " with stylist ", aptStylist.FirstName, " ", aptStylist.LastName, " at ",
                        appointmentStartTime.ToString("MM/dd/yyyy hh:mm tt"), ".");
                    calendarEvents[i] = new
                    {
                        label = appointments[i].ClientFullName,
                        dateStart = appointmentStartTime.ToString("MM/dd/yyyy HH:mm:ss"),
                        dateEnd = appointmentStartTime.AddHours(2).ToString("MM/dd/yyyy HH:mm:ss"),
                        backgroundColor = aptStylist.HexColor,
                        description
                    };
                }
                else
                {
                    // log error but continue
                }
            }

            loadIndexScreenResult.CalendarEvents = JsonSerializer.Serialize(calendarEvents);
            loadIndexScreenResult.HairStylists = stylists;
            loadIndexScreenResult.LoadIndexScreenResultStatus = LoadIndexScreenResultStatus.Success;

            return loadIndexScreenResult;
        }
    }
}
