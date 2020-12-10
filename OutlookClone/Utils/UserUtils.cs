using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using OutlookClone.Models;

namespace OutlookClone.Utils
{
    public class UserUtils
    {
        public static ContactModel GetCurrentUser(ClaimsPrincipal User, MyDbContext db)
        {
             var userGuid = ((ClaimsIdentity) User.Identity)
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            return db.Contacts.First(c => c.Guid == userGuid);
        }
    }
}
