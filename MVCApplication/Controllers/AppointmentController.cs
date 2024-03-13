using HairApplication.MVC.Logic;
using HairApplication.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AutoMapper;
using HairApplication.Logic.AppointmentSchedule;
using HairApplication.Models.Entities;
using HairApplication.Logic.Shared;

namespace HairApplication.MVC.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IMapper _mapper;
        private FirestoreProvider _firestoreProvider;
        private AppointmentScheduleHandler _appointmentScheduleHandler;

        public AppointmentController(ILogger<HomeController> logger, IMapper mappingProfile,
            FirestoreProvider firestoreProvider, AppointmentScheduleHandler appointmentScheduleHandler)
        {
            _logger = logger;
            _mapper = mappingProfile;
            _firestoreProvider = firestoreProvider;
            _appointmentScheduleHandler = appointmentScheduleHandler;
        }

        [HttpGet]
        public IActionResult Schedule()
        {
            AppointmentEntryViewModel viewModel = new AppointmentEntryViewModel();

            CancellationTokenSource source = new CancellationTokenSource();
            var stylists = _firestoreProvider.GetAll<HairStylist>(source.Token);
            viewModel.AvailableStylists = stylists.Result.ToList();

            return View(new AppointmentEntryViewModel());
        }

        [HttpPost]
        public IActionResult Schedule(AppointmentEntryViewModel viewModel)
        {
            //validation rules applied in the form
            AppointmentScheduleItem appointmentScheduleItem = _mapper.Map<AppointmentScheduleItem>(viewModel);
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