using MedicalInstitution.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalInstitution.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
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
        public IActionResult HandleError(int statuscode)
        {
            if (statuscode != 0)
            {
                ViewData["ErrorMessage"] = $"Произошла ошибка. Код ошибки: {statuscode}";
            }
            else
            {
                ViewData["ErrorMessage"] = $"Произошла ошибка. Код ошибки: 404";
            }
            return View("~/Views/Shared/HandleError.cshtml");
        }
    }
}
