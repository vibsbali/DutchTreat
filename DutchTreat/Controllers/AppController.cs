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
        private readonly ILogger logger;
        private readonly IMailService mailService;
        private readonly IDutchRepository dutchRepository;

        public AppController(ILogger<AppController> logger, IMailService mailService, IDutchRepository dutchRepository)
        {
            this.logger = logger;
            this.mailService = mailService;
            this.dutchRepository = dutchRepository;
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
            logger.LogTrace(model.ToString());
            if (ModelState.IsValid)
            {
                //Send the mail
                mailService.SendMessage(model.Name, model.Subject, model.Message);
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
            var results = dutchRepository.GetAllProducts();
            return View(results.ToList());
        }
    }
}
