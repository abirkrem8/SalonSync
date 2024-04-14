using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AutoMapper;
using SalonSync.MVC.Models;
using SalonSync.Logic.AppointmentSchedule;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using SalonSync.Logic.Load.LoadAppointmentScheduleForm;

namespace SalonSync.MVC.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IMapper _mapper;
        private FirestoreProvider _firestoreProvider;
        private AppointmentScheduleHandler _appointmentScheduleHandler;
        private LoadAppointmentScheduleFormHandler _loadAppointmentScheduleFormHandler;
        private CancellationToken _cancellationToken;

        public AppointmentController(ILogger<HomeController> logger, IMapper mappingProfile,
            FirestoreProvider firestoreProvider, AppointmentScheduleHandler appointmentScheduleHandler,
            LoadAppointmentScheduleFormHandler loadAppointmentScheduleFormHandler)
        {
            _logger = logger;
            _mapper = mappingProfile;
            _firestoreProvider = firestoreProvider;
            _appointmentScheduleHandler = appointmentScheduleHandler;
            _loadAppointmentScheduleFormHandler = loadAppointmentScheduleFormHandler;
            _cancellationToken = new CancellationTokenSource().Token;
        }

        [HttpGet]
        public IActionResult Schedule(string alert = "")
        {
            var result = _loadAppointmentScheduleFormHandler.Handle(new LoadAppointmentScheduleFormItem());
            var viewModel = _mapper.Map<AppointmentScheduleViewModel>(result);
            if (!string.IsNullOrEmpty(alert))
            {
                TempData["error-message"] = alert;
            }
            return View(viewModel);

        }

        [HttpPost]
        public IActionResult Schedule(AppointmentScheduleViewModel appointmentSubmission)
        {
            // Now we must schedule the appointment
            AppointmentScheduleItem appointmentScheduleItem = _mapper.Map<AppointmentScheduleItem>(appointmentSubmission);
            var appointmentScheduleResult = _appointmentScheduleHandler.Handle(appointmentScheduleItem);
            if (appointmentScheduleResult.AppointmentScheduleResultStatus != AppointmentScheduleResultStatus.Success)
            {
                string alert = string.Format("Unable to schedule appointment! {0}", appointmentScheduleResult.AppointmentScheduleResultErrors.FirstOrDefault().Message);
                return RedirectToAction("Schedule", "Appointment", new { alert = alert });
            }
            else
            {
                string alert = string.Format("Successfully scheduled an appointment for {0} on {1} at {2} with {3}!", appointmentScheduleResult.ClientFullName, appointmentScheduleResult.TimeOfAppointment.ToShortDateString(), appointmentScheduleResult.TimeOfAppointment.ToShortTimeString(), appointmentScheduleResult.StylistFullName);
                return RedirectToAction("Index", "Home", new { alert = alert });
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}