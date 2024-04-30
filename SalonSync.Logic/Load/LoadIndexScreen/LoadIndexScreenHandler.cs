using Google.Type;
using Microsoft.Extensions.Logging;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using SalonSync.Models.Shared;
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
            LoadIndexScreenResult result = new LoadIndexScreenResult();

            LoadIndexScreenValidator validator = new LoadIndexScreenValidator();
            var validationResult = validator.Validate(LoadIndexScreenItem);

            if (!validationResult.IsValid)
            {
                // There was an error in validation, quit now
                // log the error
                string error = string.Format("Validation Error: {0}", validationResult.Errors.FirstOrDefault());
                _logger.LogError(error);
                result.LoadIndexScreenResultStatus = LoadIndexScreenResultStatus.ValidationError;
                result.LoadIndexScreenResultErrors.Add(new Error() { Message = error });
                return result;
            }

            // Successful validation, do the handling
            try
            {
                var appointments = _firestoreProvider.GetAll<Appointment>(_cancellationToken).Result.ToList();
                var stylists = _firestoreProvider.GetAll<HairStylist>(_cancellationToken).Result.ToList();
                var calendarEvents = new object[appointments.Count()];

                // Create all of the calendar events for the smart-scheduler
                for (int i = 0; i < appointments.Count; i++)
                {
                    var aptStylist = stylists.FirstOrDefault(x => x.Id == appointments[i].HairStylist.Id);
                    if (aptStylist != null)
                    {
                        var appointmentStartTime = appointments[i].StartTimeOfAppointment.ToDateTime().ToLocalTime();
                        var description = string.Concat("This appointment is for ", appointments[i].ClientFullName,
                            " with stylist ", aptStylist.FirstName, " ", aptStylist.LastName, " at ",
                            appointmentStartTime.ToString("MM/dd/yyyy hh:mm tt"), ".");
                        // Create event object for json parsing
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
                        // no existing stylist linked to appointment, log error but continue
                        _logger.LogError(String.Format("No existing stylist: {0}", appointments[i].HairStylist.Id));
                        continue;
                    }
                }

                result.CalendarEvents = JsonSerializer.Serialize(calendarEvents);
                result.HairStylists = stylists;
                result.LoadIndexScreenResultStatus = LoadIndexScreenResultStatus.Success;
                _logger.LogInformation("Successfully loading all of the appointments into the calendar!");
                return result;
            }
            catch (Exception ex)
            {
                // Exceptions with the Firebase DB
                // log the error
                string error = string.Format("Database Error while loading appointments! {0}", ex.Message);
                _logger.LogError(error);
                result.LoadIndexScreenResultStatus = LoadIndexScreenResultStatus.DatabaseError;
                result.LoadIndexScreenResultErrors.Add(new Error { Message = error });
                return result;
            }
        }
    }
}
