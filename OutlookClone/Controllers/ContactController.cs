using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OutlookClone.Models;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Graph.Auth;
using System.Security.Claims;
using X.PagedList;

namespace OutlookClone.Controllers
{
    public class ContactController : Controller
    {
        private readonly MyDbContext db;
        public ContactController(MyDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ImportContactsFromAzure2Db().Wait();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.LastNameSortParm = sortOrder == "last_name" ? "last_name_desc" : "last_name";
            ViewBag.IdSortParm = sortOrder == "id" ? "id_desc" : "id";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var contacts = from s in db.Contacts
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                contacts = contacts.Where(s => (s.FirstName+" "+s.LastName).Contains(searchString));
            }
            contacts = sortOrder switch
            {
                "name_desc" => contacts.OrderByDescending(s => s.FirstName),
                "last_name" => contacts.OrderBy(s => s.LastName),
                "last_name_desc" => contacts.OrderByDescending(s => s.LastName),
                "id" => contacts.OrderBy(s => s.Id),
                "id_desc" => contacts.OrderByDescending(s => s.Id),
                _ => contacts.OrderBy(s => s.FirstName),
            };
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(contacts.ToPagedList(pageNumber, pageSize));
        }

        public async Task ImportContactsFromAzure2Db()
        {
            //Build a client application.
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                           .Create("b94d1d3d-3f7d-4f83-bb4a-751f46b72d8e")
                           .WithTenantId("2e18f15a-865a-4f40-bc98-c9fb24d156af")
                           .WithClientSecret("RW-qrUhAk5_Fqh45~_s95xaC_7E2iJAYY2")
                           .Build();
            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);
            // Create a new instance of GraphServiceClient with the authentication provider.
            GraphServiceClient graphClient = new GraphServiceClient(authProvider);
            var contactsAzure = await graphClient.Users.Request().GetAsync();
            var contactsDB = db.Contacts.ToList();
            foreach (var conA in contactsAzure)
            {

                bool newUser = true;
                foreach (var conD in contactsDB)
                    if (conA.Id == conD.Guid)
                        newUser = false;
                if(newUser)
                    db.Contacts.Add((ContactModel)conA);
            }
            db.SaveChanges();
        }

    }
}