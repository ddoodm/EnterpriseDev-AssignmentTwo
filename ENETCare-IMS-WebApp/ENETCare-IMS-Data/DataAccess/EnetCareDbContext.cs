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
using System.IO;

namespace ENETCare.IMS.Data.DataAccess
{
    public class EnetCareDbContext : IdentityDbContext<EnetCareUser>
    {
        public EnetCareDbContext()
            : base("name=EnetCareImsDatabase")
        {
            Configuration.LazyLoadingEnabled = true;
        }

        public DbSet<Intervention> Interventions { get; set; }
        public DbSet<InterventionType> InterventionTypes { get; set; }
        public DbSet<InterventionApproval> InterventionApprovals { get; set; }
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