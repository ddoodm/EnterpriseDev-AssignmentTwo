using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

using ENETCare.IMS.Users;
using ENETCare.IMS.Interventions;
using ENETCare.IMS;
using ENETCare.IMS.Data.DataAccess;

namespace ENETCare_IMS_WebApp.Models
{
    public class EnetCareRole : IdentityRole
    {
        public EnetCareRole()
            : base()
        { }

        public EnetCareRole(string name)
            : base(name)
        { }
    }

    public class EnetCareRoleManager : RoleManager<EnetCareRole>, IDisposable
    {
        public EnetCareRoleManager(RoleStore<EnetCareRole> store)
            : base(store)
        { }

        /// <summary>
        /// Called by OWIN to create the Role Manager.
        /// Delegate set in SetupAuth
        /// </summary>
        public static EnetCareRoleManager Create(
            IdentityFactoryOptions<EnetCareRoleManager> options,
            IOwinContext context)
        {
            return new EnetCareRoleManager(
                new RoleStore<EnetCareRole>(context.Get<EnetCareDbContext>())
                );
        }
    }
}