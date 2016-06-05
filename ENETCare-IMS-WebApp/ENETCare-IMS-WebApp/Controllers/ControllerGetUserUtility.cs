using ENETCare.IMS.Data.DataAccess;
using ENETCare.IMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNet.Identity;

namespace ENETCare_IMS_WebApp.Controllers
{
    public static class ControllerGetUserUtility
    {
        public static T GetSessionUser<T>(EnetCareDbContext context, IPrincipal user) where T : EnetCareUser
        {
            var repo = new UserRepo(context);
            return repo.GetUserById<T>(user.Identity.GetUserId());
        }

        public static IInterventionApprover GetSessionApproverUser(EnetCareDbContext context, IPrincipal user)
        {
            if (user.IsInRole("SiteEngineer"))
                return GetSessionUser<SiteEngineer>(context, user);
            if (user.IsInRole("Manager"))
                return GetSessionUser<Manager>(context, user);

            return null;
        }

        public static SiteEngineer GetSessionSiteEngineer(EnetCareDbContext context, IPrincipal user)
        {
            return GetSessionUser<SiteEngineer>(context, user);
        }
    }
}