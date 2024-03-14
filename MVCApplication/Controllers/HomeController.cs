using AutoMapper;
using HairApplication.Logic.LoadIndexScreen;
using HairApplication.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HairApplication.MVC.Controllers
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

        [HttpGet]
        public IActionResult Index()
        {
            var result = _loadIndexScreenHandler.Handle(new LoadIndexScreenItem());
            if (result.LoadIndexScreenResultStatus != LoadIndexScreenResultStatus.Success)
            {
                // handle error
                // log error
                // redirect to error page
            }
            IndexViewModel viewModel = _mapper.Map<IndexViewModel>(result);
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