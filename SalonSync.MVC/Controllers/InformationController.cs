using SalonSync.MVC.Logic;
using SalonSync.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AutoMapper;
using SalonSync.Logic.AppointmentSchedule;
using SalonSync.Logic.Shared;
using SalonSync.Models.Entities;
using SalonSync.Logic.Load.LoadStylistInformation;
using SalonSync.Logic.Load.LoadClientInformation;

namespace SalonSync.MVC.Controllers
{
    public class InformationController : Controller
    {
        private readonly ILogger<InformationController> _logger;
        private IMapper _mapper;
        private LoadStylistInformationHandler _loadStylistInformationHandler;
        private LoadClientInformationHandler _loadClientInformationHandler;

        public InformationController(ILogger<InformationController> logger, IMapper mappingProfile,
            FirestoreProvider firestoreProvider, LoadStylistInformationHandler loadStylistInformationHandler,
            LoadClientInformationHandler loadClientInformationHandler)
        {
            _logger = logger;
            _mapper = mappingProfile;
            _loadStylistInformationHandler = loadStylistInformationHandler;
            _loadClientInformationHandler = loadClientInformationHandler;
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

        [HttpGet]
        public IActionResult Client(string clientId)
        {
            LoadClientInformationItem loadClientInformationItem  = new LoadClientInformationItem() { ClientId = clientId };
            var result = _loadClientInformationHandler.Handle(loadClientInformationItem);

            if (result != null && result.LoadClientInformationResultStatus == LoadClientInformationResultStatus.Success)
            {
                ClientInformationViewModel viewModel = _mapper.Map<ClientInformationViewModel>(result);
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