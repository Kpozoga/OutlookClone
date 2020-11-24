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

namespace OutlookClone.Controllers
{
    public class ContactController : Controller
    {
        private readonly MyDbContext db;
        public ContactController(MyDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            ImportContactsFromAzure2Db().Wait();
            return View(db.Contacts.ToList());
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
                    if (conA.Id != conD.Guid)
                        newUser = false;
                if(newUser)
                    db.Contacts.Add((ContactModel)conA);
            }
            db.SaveChanges();
        }

    }
}