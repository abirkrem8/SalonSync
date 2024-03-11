using HairApplication.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HairApplication.MVC.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public AppointmentController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Schedule()
        {
            return View(new AppointmentEntryViewModel());
        }

        [HttpPost]
        public IActionResult Schedule(AppointmentEntryViewModel viewModel)
        {
            //validation



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