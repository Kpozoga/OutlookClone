using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OutlookClone.Models;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Graph.Auth;
using X.PagedList;

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
            ImportContactsFromAzure2Db().Wait();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
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
                _ => contacts.OrderBy(s => s.FirstName),
            };
            
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

        public IActionResult Detail(int id)
        {
            throw new NotImplementedException();
        }
    }
}