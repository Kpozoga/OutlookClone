using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OutlookClone.Models;

namespace OutlookClone.Controllers
{
    public class MailController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            List<MailModel> mailList = new List<MailModel>
            {
                new MailModel
                {
                    Subject = "Test Subject 1",
                    Body = "Test Body 1, Test Body 1, Test Body 1, Test Body 1, Test Body 1, ",
                    From = "test1@example.com",
                    Date = DateTime.Now,
                }, 
                new MailModel
                {
                    Subject = "Test Subject 2",
                    Body = "Test Body 2, Test Body 2, Test Body 2, Test Body 2, Test Body 1, ",
                    From = "test2@example.com",
                    Date = DateTime.Now,
                }, 
            };

            return View(mailList);
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreatePost(MailModel mm)
        {
            mm.Date = DateTime.Now;
            // TODO: save mm to database
            
            // always redirect from Post endpoint to avoid double submission
            return RedirectToAction("Index");
        }
    }
}