using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OutlookClone.Models;

namespace OutlookClone.Controllers
{
    public class MailController : Controller
    {
        private readonly MyDbContext db;

        public MailController(MyDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return Forbid();
            }

            var userGuid = ((ClaimsIdentity) User.Identity)
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            var currentUser = db.Contacts.First(c => c.Guid == userGuid);

            var mails = db.Mails.Where(mail => mail.To.Contains(currentUser)).ToList();

            return View(mails);
        }

        [HttpGet]
        public IActionResult Detail(int id)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return Forbid();
            }

            var userGuid = ((ClaimsIdentity) User.Identity)
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            var currentUser = db.Contacts.First(c => c.Guid == userGuid);

            var mail = db.Mails.First(model => model.Id == id);

            // if (!mail.To.Contains(currentUser))
            // {
            //     return Forbid();
            // }

            return View(mail);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return Forbid();
            }

            ViewData["ContactList"] = db.Contacts.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult CreatePost(MailModel mm)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return Forbid();
            }

            var userGuid = ((ClaimsIdentity) User.Identity)
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

            mm.FromId = db.Contacts.First(contact => contact.Guid == userGuid).Id;
            var to = Request.Form["to"].ToList().Select(int.Parse);
            mm.To = db.Contacts.Where(c => to.Contains(c.Id)).ToList();
            mm.Date = DateTime.Now;
            db.Mails.Add(mm);
            db.SaveChanges();

            // always redirect from Post endpoint to avoid double submission
            return RedirectToAction("Index");
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var mail = new MailModel{ Id = id };
            db.Mails.Attach(mail);
            db.Mails.Remove(mail);
            db.SaveChanges();
            
            return RedirectToAction("Index");
        }
    }
}