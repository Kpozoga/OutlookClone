using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OutlookClone.Models;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using OutlookClone.Services;
using X.PagedList;

namespace OutlookClone.Controllers
{
    public class MailController : Controller
    {
        private readonly MyDbContext db;
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration configuration;
        
        private const int PageSize = 10;

        public MailController(MyDbContext db, IWebHostEnvironment env, IConfiguration configuration)
        {
            this.db = db;
            this.env = env;
            this.configuration = configuration;
        }
        
        [HttpGet]
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return Forbid();
            }
            if (!Utils.UserUtils.GetCurrentUser(User, db).IsActive) return Forbid();
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

            var mail = db.Mails
                .Include(model => model.Attachments)
                .First(model => model.Id == id);

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
        public async Task<IActionResult> CreatePost(MailModel mm)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return Forbid();
            }

            var currentUser = Utils.UserUtils.GetCurrentUser(User, db);

            mm.FromId = currentUser.Id;
            var to = Request.Form["to"].ToList().Select(int.Parse);
            mm.To = db.Contacts.Where(c => to.Contains(c.Id)).ToList();
            db.Mails.Add(mm);
            
            # region Upload File         
            var objBlobService = new BlobStorageService(configuration);

            foreach (var file in Request.Form.Files)
            {
                var attachment = new AttachmentModel();
                
                var fileStream = file.OpenReadStream();
                var mimeType = file.ContentType;
                var fileData = new byte[file.Length];

                fileStream.Read(fileData, 0, (int)file.Length);

                attachment.FileName = file.FileName;
                attachment.FileUri = objBlobService.UploadFileToBlob(file.FileName, fileData, mimeType);
                
                mm.Attachments.Add(attachment);
            }

            # endregion
            
            db.SaveChanges();
            
            _ = Utils.UserUtils.SendEmailNotification(currentUser, mm, null, Url.Action("Detail", "Mail", null, Request.Scheme));
            _ = Utils.UserUtils.SendSmsNotification(currentUser, mm, null, Url.Action("Detail", "Mail", null, Request.Scheme));

            var message = "";
            var recipientList = "";
            var withAttachments = "";
                        
            // send request to NotificationService.API
            var jsonObject = (dynamic)new JsonObject();
            jsonObject.content = message;
            jsonObject.contentType = "string";
            jsonObject.recepientsList = new List<string> { };
            jsonObject.withAttachments = false;

            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            var client = new HttpClient();
            // TODO: fill with real api-key
            client.DefaultRequestHeaders.Add("x-api-key", "f5e4713d-e636-48c0-bb33-b478040dd047");
            var response = await client.PostAsync(
                "https://mini-notification-service.azurewebsites.net/notifications",
                content
            );
            var responseString = await response.Content.ReadAsStringAsync(); // comment this out?

            // always redirect from Post endpoint to avoid double submission
            return RedirectToAction("Index");
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            // that would be a f***ing one liner in Django, I can't even control cascading nor 
            var mail = db.Mails
                .Where(m => m.Id == id)
                .Include(m => m.Attachments)
                .First();
            foreach (var attachment in mail.Attachments)
            {
                var objBlob = new BlobStorageService(configuration);
                objBlob.DeleteBlobData(attachment.FileUri);
                db.Attachments.Remove(attachment);
            }
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