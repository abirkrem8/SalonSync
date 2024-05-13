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
        public IActionResult Stylist(string stylistId, string alert = "")
        {
            
            StylistDetailViewModel model = new StylistDetailViewModel();
            LoadStylistInformationItem loadStylistInformationItem = new LoadStylistInformationItem() { HairStylistId = stylistId };
            LoadStylistInformationResult result = _loadStylistInformationHandler.Handle(loadStylistInformationItem);

            if (result.LoadStylistInformationResultStatus != LoadStylistInformationResultStatus.Success)
            {
                string err = String.Format("An error occurred while loading this screen - {0}",
                    result.LoadStylistInformationResultErrors.FirstOrDefault().Message);
                TempData["error-message"] = err;
            }
            else
            {
                model = _mapper.Map<StylistDetailViewModel>(result);
                if (!string.IsNullOrEmpty(alert))
                {
                    TempData["success-message"] = alert;
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Client(string clientId, string successAlert = "",string failureAlert="")
        {
            LoadClientInformationItem loadClientInformationItem  = new LoadClientInformationItem() { ClientId = clientId };
            var result = _loadClientInformationHandler.Handle(loadClientInformationItem);

            if (result != null && result.LoadClientInformationResultStatus == LoadClientInformationResultStatus.Success)
            {
                ClientInformationViewModel viewModel = _mapper.Map<ClientInformationViewModel>(result);
                if (!string.IsNullOrEmpty(successAlert))
                {
                    TempData["success-message"] = successAlert;
                }
                if (!string.IsNullOrEmpty(failureAlert))
                {
                    TempData["error-message"] = failureAlert;
                }
                return View(viewModel);
            }
            else
            {
                // log error
                string err = String.Format("An error occurred while loading this screen - {0}",
                    result.LoadClientInformationResultErrors.FirstOrDefault().Message);
                TempData["error-message"] = err;
                return View(new ClientInformationViewModel());
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}