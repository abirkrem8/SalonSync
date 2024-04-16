using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SalonSync.Logic.Load.LoadIndexScreen;
using SalonSync.MVC.Models;
using System.Diagnostics;

namespace SalonSync.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private LoadIndexScreenHandler _loadIndexScreenHandler;
        private IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, LoadIndexScreenHandler loadIndexScreenHandler,
            IMapper mapper)
        {
            _logger = logger;
            _loadIndexScreenHandler = loadIndexScreenHandler;
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