using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OutlookClone.Models;
using X.PagedList;

namespace OutlookClone.Controllers
{
    public class GroupController : Controller
    {
        private readonly MyDbContext db;
        private const int PageSize = 10;

        public GroupController(MyDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
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
            var groups = from g in db.Groups select g;
            if (!string.IsNullOrEmpty(searchString))
            {
                groups = groups.Where(c => (c.GroupName).Contains(searchString));
            }
            groups = sortOrder switch
            {
                "name_desc" => groups.OrderByDescending(g => g.GroupName),
                "id" => groups.OrderBy(g => g.Id),
                "id_desc" => groups.OrderByDescending(g => g.Id),
                _ => groups.OrderBy(g => g.GroupName),
            };
            
            return View(groups.ToPagedList(page ?? 1, PageSize));
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
        public IActionResult CreatePost(GroupModel gm)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return Forbid();
            }

            var userGuid = ((ClaimsIdentity)User.Identity)
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

            gm.GroupOwnerID = db.Contacts.First(contact => contact.Guid == userGuid).Id;
            var to = Request.Form["to"].ToList().Select(int.Parse);
            gm.GroupMembers = db.Contacts.Where(c => to.Contains(c.Id)).ToList();
            db.Groups.Add(gm);
            db.SaveChanges();

            // always redirect from Post endpoint to avoid double submission
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Detail(int id)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return Forbid();
            }

            var userGuid = ((ClaimsIdentity)User.Identity)
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            var currentUser = db.Contacts.First(c => c.Guid == userGuid);

            var group = db.Groups.First(model => model.Id == id);

            // if (!mail.To.Contains(currentUser))
            // {
            //     return Forbid();
            // }

            return View(group);
        }
    }
}