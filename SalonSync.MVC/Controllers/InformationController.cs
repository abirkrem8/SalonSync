using SalonSync.MVC.Logic;
using SalonSync.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AutoMapper;
using SalonSync.Logic.AppointmentSchedule;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using SalonSync.Logic.Load.LoadStylistInformation;

namespace SalonSync.MVC.Controllers
{
    public class InformationController : Controller
    {
        private readonly ILogger<InformationController> _logger;
        private IMapper _mapper;
        private FirestoreProvider _firestoreProvider;
        private LoadStylistInformationHandler _loadStylistInformationHandler;
        private CancellationToken _cancellationToken;

        public InformationController(ILogger<InformationController> logger, IMapper mappingProfile,
            FirestoreProvider firestoreProvider, LoadStylistInformationHandler loadStylistInformationHandler)
        {
            _logger = logger;
            _mapper = mappingProfile;
            _firestoreProvider = firestoreProvider;
            _loadStylistInformationHandler = loadStylistInformationHandler;
            _cancellationToken = new CancellationTokenSource().Token;
        }

        [HttpGet]
        public IActionResult Stylist(string stylistId)
        {
            LoadStylistInformationItem loadStylistInformationItem = new LoadStylistInformationItem() { HairStylistId = stylistId };
            LoadStylistInformationResult result = _loadStylistInformationHandler.Handle(loadStylistInformationItem);

            if (result != null && result.LoadStylistInformationResultStatus == LoadStylistInformationResultStatus.Success)
            {
                StylistDetailViewModel viewModel = _mapper.Map<StylistDetailViewModel>(result);
                return View(viewModel);
            }
            else
            {
                // log error
                return RedirectToAction("Error", "Home");
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}