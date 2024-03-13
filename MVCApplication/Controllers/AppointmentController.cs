using HairApplication.MVC.Logic;
using HairApplication.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AutoMapper;
using HairApplication.Logic.AppointmentSchedule;
using HairApplication.Models.Entities;
using HairApplication.Logic.Shared;
using HairApplication.Logic.AppointmentConfirmation;

namespace HairApplication.MVC.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IMapper _mapper;
        private FirestoreProvider _firestoreProvider;
        private AppointmentScheduleHandler _appointmentScheduleHandler;
        private AppointmentConfirmationHandler _appointmentConfirmationHandler;
        private CancellationToken _cancellationToken;

        public AppointmentController(ILogger<HomeController> logger, IMapper mappingProfile,
            FirestoreProvider firestoreProvider, AppointmentScheduleHandler appointmentScheduleHandler,
            AppointmentConfirmationHandler appointmentConfirmationHandler)
        {
            _logger = logger;
            _mapper = mappingProfile;
            _firestoreProvider = firestoreProvider;
            _appointmentScheduleHandler = appointmentScheduleHandler;
            _appointmentConfirmationHandler = appointmentConfirmationHandler;
            _cancellationToken = new CancellationTokenSource().Token;
        }

        [HttpGet]
        public IActionResult Schedule()
        {
            AppointmentEntryViewModel viewModel = new AppointmentEntryViewModel();

            CancellationTokenSource source = new CancellationTokenSource();
            var stylists = _firestoreProvider.GetAll<HairStylist>(source.Token);

            if (stylists != null && stylists.Result.Count > 0)
            {
                viewModel.AvailableStylists = stylists.Result.ToList();

                return View(viewModel);
            }
            else
            {
                _logger.LogError("No Available Stylists!");
                return RedirectToAction("Error");
            }
        }

        public IActionResult Confirm(AppointmentEntryViewModel appointmentSubmission)
        {
            // The appointment form has been filled out and now we need to validate the submission
            // and show the user the submitted information before creating it in our database.
            var item = _mapper.Map<AppointmentConfirmationItem>(appointmentSubmission);
            var result = _appointmentConfirmationHandler.Handle(item);

            var viewModel = _mapper.Map<AppointmentConfirmationViewModel>(result);
            return View(viewModel);
        }

        public IActionResult Confirm(AppointmentConfirmationViewModel appointmentConfirmation)
        {
            // Now we must schedule the appointment
            AppointmentScheduleItem appointmentScheduleItem = _mapper.Map<AppointmentScheduleItem>(appointmentConfirmation);
            var appointmentScheduleResult = _appointmentScheduleHandler.Handle(appointmentScheduleItem);

            return View();
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