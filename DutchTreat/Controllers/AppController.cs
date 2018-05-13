using System.Linq;
using DutchTreat.Data;
using DutchTreat.Services;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers
{
    public class AppController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMailService _mailService;
        private readonly IDutchRepository _dutchRepository;

        public AppController(ILogger<AppController> logger, IMailService mailService, IDutchRepository dutchRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _dutchRepository = dutchRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            _logger.LogTrace(model.ToString());
            if (ModelState.IsValid)
            {
                //Send the mail
                _mailService.SendMessage(model.Name, model.Subject, model.Message);
                ViewBag.UserMessage = "Mail Sent";
                ModelState.Clear();
            }
            
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Title = "About us";
            return View();
        }

        [Authorize]
        public IActionResult Shop()
        {
            var results = _dutchRepository.GetAllProducts();
            return View(results.ToList());
        }
    }
}
