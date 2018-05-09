﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DutchTreat.Services;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace DutchTreat.Controllers
{
    public class AppController : Controller
    {
        private readonly ILogger logger;
        private readonly IMailService mailService;

        public AppController(ILogger<AppController> logger, IMailService mailService)
        {
            this.logger = logger;
            this.mailService = mailService;
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
    }
}
