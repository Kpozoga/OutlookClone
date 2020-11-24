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
            if(User.Identity.IsAuthenticated)
            {
                var myId = ((ClaimsIdentity)User.Identity).FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
                var me = db.Contacts.Where(c => c.Guid == myId).First();
                db.Entry(me).Collection(c => c.Mails).Load();
                //var myMails = db.Mails.Where(m => m.To.Contains(me));
                return View(me.Mails.ToList());
            }
            return View(db.Mails.ToList());
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
            //TODO: save mm to database  
            mm.From = "test1@example.com";
            if(User.Identity.IsAuthenticated)
            {
                mm.To = new List<ContactModel>();
                var myId = ((ClaimsIdentity)User.Identity).FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
                var me = db.Contacts.Where(c => c.Guid == myId).First();
                mm.From = me.FirstName + " " + me.LastName;
                mm.To.Add(me);
            }
            db.Mails.Add(mm);
            db.SaveChanges();
            // always redirect from Post endpoint to avoid double submission
            return RedirectToAction("Index");

        }
    }
}