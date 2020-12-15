using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OutlookClone.Models;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Graph.Auth;
using X.PagedList;
using OutlookClone.Utils;

namespace OutlookClone.Controllers
{
    public class ContactController : Controller
    {
        private readonly MyDbContext db;
        private const int PageSize = 10;
        
        public ContactController(MyDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return Forbid();
            }
            var currentUser = UserUtils.GetCurrentUser(User, db);
            if (!currentUser.IsActive&&!currentUser.IsAdmin)
                return Forbid();
            ImportContactsFromAzure2Db().Wait();
            ViewBag.CurrentUser = currentUser;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.LastNameSortParm = sortOrder == "last_name" ? "last_name_desc" : "last_name";
            ViewBag.IdSortParm = sortOrder == "id" ? "id_desc" : "id";
            ViewBag.JoinDateSortParm = sortOrder == "date" ? "date_desc" : "date";
            ViewBag.SortOrder = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var contacts = from c in db.Contacts select c;
            if (!string.IsNullOrEmpty(searchString))
            {
                contacts = contacts.Where(c => (c.FirstName+" "+c.LastName).Contains(searchString));
            }
            contacts = sortOrder switch
            {
                "name_desc" => contacts.OrderByDescending(s => s.FirstName),
                "last_name" => contacts.OrderBy(s => s.LastName),
                "last_name_desc" => contacts.OrderByDescending(s => s.LastName),
                "id" => contacts.OrderBy(s => s.Id),
                "id_desc" => contacts.OrderByDescending(s => s.Id),
                "date" => contacts.OrderBy(s => s.JoinDate),
                "date_desc" => contacts.OrderByDescending(s => s.JoinDate),
                _ => contacts.OrderBy(s => s.FirstName),
            };

            if (!currentUser.IsAdmin)
                contacts = contacts.Where(c => c.IsActive);

            return View(contacts.ToPagedList(page ?? 1, PageSize));
        }

        public async Task ImportContactsFromAzure2Db()
        {
            //Build a client application.
            var confidentialClientApplication = ConfidentialClientApplicationBuilder
                           .Create("b94d1d3d-3f7d-4f83-bb4a-751f46b72d8e")
                           .WithTenantId("2e18f15a-865a-4f40-bc98-c9fb24d156af")
                           .WithClientSecret("RW-qrUhAk5_Fqh45~_s95xaC_7E2iJAYY2")
                           .Build();
            var authProvider = new ClientCredentialProvider(confidentialClientApplication);
            // Create a new instance of GraphServiceClient with the authentication provider.
            var graphClient = new GraphServiceClient(authProvider);
            var contactsAzure = await graphClient.Users.Request().GetAsync();
            var contactsDB = db.Contacts.ToList();
            foreach (var conA in contactsAzure)
            {

                var newUser = contactsDB.All(conD => conA.Id != conD.Guid);

                if (newUser)
                {
                    db.Contacts.Add((ContactModel)conA);
                }
            }
            db.SaveChanges();
        }


        public IActionResult Disenable(int id)
        {
            if (!UserUtils.GetCurrentUser(User,db).IsAdmin)
            {
                return Forbid();
            }
            var user = db.Contacts.First(c => c.Id == id);
            user.IsActive = !user.IsActive;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Road2Admin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult BecomeAdmin(string pass)
        {
            if(pass== "'?{_4gGBb>}mz6?A")
            {
                var me = UserUtils.GetCurrentUser(User, db);
                me.IsAdmin = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Road2Admin");
        }
        public IActionResult Profile()
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return Forbid();
            }
            var currentUser = UserUtils.GetCurrentUser(User, db);

            return View(currentUser);
        }
        [HttpPost]
        public IActionResult ProfilePost(ContactModel usr)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return Forbid();
            }
            var currentUser = UserUtils.GetCurrentUser(User, db);
            currentUser.Email = usr.Email;
            currentUser.PhoneNumber = usr.PhoneNumber;
            db.Update(currentUser);
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}