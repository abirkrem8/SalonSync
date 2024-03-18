using SalonSync.MVC.Logic;
using SalonSync.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AutoMapper;
using SalonSync.Logic.AppointmentSchedule;
using SalonSync.Logic.AppointmentConfirmation;
using SalonSync.MVC.Models;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;

namespace SalonSync.MVC.Controllers
{
    public class InformationController : Controller
    {
        private readonly ILogger<InformationController> _logger;
        private IMapper _mapper;
        private FirestoreProvider _firestoreProvider;
        private CancellationToken _cancellationToken;

        public InformationController(ILogger<InformationController> logger, IMapper mappingProfile,
            FirestoreProvider firestoreProvider)
        {
            _logger = logger;
            _mapper = mappingProfile;
            _firestoreProvider = firestoreProvider;
            _cancellationToken = new CancellationTokenSource().Token;
        }

        [HttpGet]
        public IActionResult Stylist(string stylistId)
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}