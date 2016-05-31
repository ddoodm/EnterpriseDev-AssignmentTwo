using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using ENETCare.IMS.Users;
using ENETCare.IMS.Interventions;
using ENETCare.IMS;
using ENETCare.IMS.Data.DataAccess;

namespace ENETCare_IMS_WebApp.Models
{/*
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

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("EnetCareImsDatabase", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Intervention> Interventions { get; set; }
        public DbSet<InterventionType> InterventionTypes { get; set; }
        public DbSet<InterventionApproval> InterventionApprovals { get; set; }
        public DbSet<EnetCareUser> UserProfiles { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<District> Districts { get; set; }
    }*/
}