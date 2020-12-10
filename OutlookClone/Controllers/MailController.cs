using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OutlookClone.Models;
using X.PagedList;

namespace OutlookClone.Controllers
{
    public class MailController : Controller
    {
        private readonly MyDbContext db;
        private const int PageSize = 10;

        public MailController(MyDbContext db)
        {
            this.db = db;
        }
        
        [HttpGet]
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return Forbid();
            }
            
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SubjectSortParm = string.IsNullOrEmpty(sortOrder) ? "subject_desc" : "";
            ViewBag.SnippetSortParm = sortOrder == "snippet" ? "snippet_desc" : "snippet";
            ViewBag.FromSortParm = sortOrder == "from" ? "from_desc" : "from";
            ViewBag.DateSortParm = sortOrder == "date" ? "date_desc" : "date";
            ViewBag.ReadSortParm = sortOrder == "read" ? "read_desc" : "read";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            
            var userGuid = ((ClaimsIdentity) User.Identity)
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            var currentUser = db.Contacts.First(c => c.Guid == userGuid);

            var mails = db.Mails.Where(mail => mail.To.Contains(currentUser));
            
            if (!string.IsNullOrEmpty(searchString))
            {
                mails = mails.Where(c => (c.Subject).Contains(searchString));
            }
            
            mails = sortOrder switch
            {
                "subject_desc" => mails.OrderByDescending(m => m.Subject),
                "snippet" => mails.OrderBy(m => m.Body),
                "snippet_desc" => mails.OrderByDescending(m => m.Body),
                "from" => mails.OrderBy(m => m.FromId),
                "from_desc" => mails.OrderByDescending(m => m.FromId),
                "date" => mails.OrderBy(m => m.Date),
                "date_desc" => mails.OrderByDescending(m => m.Date),
                "read" => mails.OrderBy(m => m.Read),
                "read_desc" => mails.OrderByDescending(m => m.Read),
                _ => mails.OrderByDescending(m => m.Date),
            };
            
            return View(mails.ToPagedList(page ?? 1, PageSize));
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

        [HttpPost]
        public IActionResult SetRead()
        {
            var mailId = int.Parse(Request.Form["mail_id"]);
            var read = Request.Form["read"] == "true";
            db.Mails.First(m => m.Id == mailId).Read = read;
            db.SaveChanges();
            
            return RedirectToAction("Index");
        }
    }
}