using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;
using System.Security.Claims;

namespace ENETCare.IMS.Data.DataAccess
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class SiteEngineerAppUser : ApplicationUser
    {
        public virtual SiteEngineer siteEngineerProfile { get; set; }
    }

    public class EnetCareDbContext : IdentityDbContext<ApplicationUser>
    {
        public EnetCareDbContext()
            : base("EnetCareImsDatabase")
        { }

        public DbSet<Intervention> Interventions { get; set; }
        public DbSet<InterventionType> InterventionTypes { get; set; }
        public DbSet<InterventionApproval> InterventionApprovals { get; set; }
        public DbSet<EnetCareUser> UserProfiles { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<District> Districts { get; set; }

        /// <summary>
        /// Required for ASP Identity delegate
        /// </summary>
        public static EnetCareDbContext Create()
        {
            return new EnetCareDbContext();
        }
    }
}