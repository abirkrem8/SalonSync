using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AutoMapper;
using SalonSync.MVC.Models;
using SalonSync.Logic.AppointmentSchedule;
using SalonSync.Logic.Shared;
using SalonSync.Logic.AppointmentConfirmation;
using SalonSync.Models.Entities;
using HairApplication.Logic.LoadAppointmentScheduleForm;

namespace SalonSync.MVC.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IMapper _mapper;
        private FirestoreProvider _firestoreProvider;
        private AppointmentScheduleHandler _appointmentScheduleHandler;
        private AppointmentConfirmationHandler _appointmentConfirmationHandler;
        private LoadAppointmentScheduleFormHandler _loadAppointmentScheduleFormHandler;
        private CancellationToken _cancellationToken;

        public AppointmentController(ILogger<HomeController> logger, IMapper mappingProfile,
            FirestoreProvider firestoreProvider, AppointmentScheduleHandler appointmentScheduleHandler,
            AppointmentConfirmationHandler appointmentConfirmationHandler, LoadAppointmentScheduleFormHandler loadAppointmentScheduleFormHandler)
        {
            _logger = logger;
            _mapper = mappingProfile;
            _firestoreProvider = firestoreProvider;
            _appointmentScheduleHandler = appointmentScheduleHandler;
            _appointmentConfirmationHandler = appointmentConfirmationHandler;
            _loadAppointmentScheduleFormHandler = loadAppointmentScheduleFormHandler;
            _cancellationToken = new CancellationTokenSource().Token;
        }

        [HttpGet]
        public IActionResult Schedule()
        {
            var result = _loadAppointmentScheduleFormHandler.Handle(new LoadAppointmentScheduleFormItem());
            var viewModel = _mapper.Map<AppointmentEntryViewModel>(result);
            return View(viewModel);

        }

        public IActionResult Confirm(AppointmentEntryViewModel appointmentSubmission)
        {
            // The appointment form has been filled out and now we need to validate the submission
            // and show the user the submitted information before creating it in our database.
            var item = _mapper.Map<AppointmentConfirmationItem>(appointmentSubmission);
            var result = _appointmentConfirmationHandler.Handle(item);
            if (result.AppointmentConfirmationResultStatus == AppointmentConfirmationResultStatus.StylistAlreadyBooked)
            {
                // Do something
            }
            var viewModel = _mapper.Map<AppointmentConfirmationViewModel>(result);
            return View(viewModel);
        }

        public IActionResult Submit(AppointmentConfirmationViewModel appointmentConfirmation)
        {
            // Now we must schedule the appointment
            AppointmentScheduleItem appointmentScheduleItem = _mapper.Map<AppointmentScheduleItem>(appointmentConfirmation);
            var appointmentScheduleResult = _appointmentScheduleHandler.Handle(appointmentScheduleItem);

            return RedirectToAction("Index", "Home");
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