using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SalonSync.Logic.Load.LoadClientList;
using SalonSync.Logic.Load.LoadIndexScreen;
using SalonSync.MVC.Models;
using System.Diagnostics;

namespace SalonSync.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private LoadIndexScreenHandler _loadIndexScreenHandler;
        private LoadClientListHandler _loadClientListHandler;
        private IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, LoadIndexScreenHandler loadIndexScreenHandler,
            IMapper mapper, LoadClientListHandler loadClientListHandler)
        {
            _logger = logger;
            _loadIndexScreenHandler = loadIndexScreenHandler;
            _loadClientListHandler = loadClientListHandler;
            _mapper = mapper;
        }
        public IActionResult Landing()
        {
            return View();
        }
        public IActionResult Index(string alert = "")
        {
            IndexViewModel model = new IndexViewModel();
            var result = _loadIndexScreenHandler.Handle(new LoadIndexScreenItem());

            if (result.LoadIndexScreenResultStatus != LoadIndexScreenResultStatus.Success)
            {
                string err = String.Format("An error occurred while loading this screen - {0}",
                    result.LoadIndexScreenResultErrors.FirstOrDefault().Message);
                TempData["error-message"] = err;
            }
            else
            {
                model = _mapper.Map<IndexViewModel>(result);
                if (!string.IsNullOrEmpty(alert))
                {
                    TempData["success-message"] = alert;
                }
            }

            return View(model);
        }

        public IActionResult ClientList(string alert = "")
        {
            ClientListViewModel model = new ClientListViewModel();

            var result = _loadClientListHandler.Handle(new LoadClientListItem());
            if (result.LoadClientListResultStatus != LoadClientListResultStatus.Success)
            {
                string err = String.Format("An error occurred while loading this screen - {0}",
                    result.LoadClientListResultErrors.FirstOrDefault().Message);
                TempData["error-message"] = err;
            }
            else
            {
                model = _mapper.Map<ClientListViewModel>(result);
                if (!string.IsNullOrEmpty(alert))
                {
                    TempData["success-message"] = alert;
                }
            }
            return View(model);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}