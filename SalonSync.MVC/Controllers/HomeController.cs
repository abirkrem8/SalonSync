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
            var result = _loadIndexScreenHandler.Handle(new LoadIndexScreenItem());
            if (result.LoadIndexScreenResultStatus != LoadIndexScreenResultStatus.Success)
            {
                // handle error
                // log error
                // redirect to error page
            }

            IndexViewModel viewModel = _mapper.Map<IndexViewModel>(result);
            if (!string.IsNullOrEmpty(alert))
            {
                TempData["success-message"] = alert;
            }
            return View(viewModel);
        }

        public IActionResult ClientList()
        {
            var result = _loadClientListHandler.Handle(new LoadClientListItem());
            if (result.LoadClientListResultStatus != LoadClientListResultStatus.Success)
            {
                // handle error
                // log error
                // redirect to error page
            }

            ClientListViewModel viewModel = _mapper.Map<ClientListViewModel>(result);

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}