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
    public class EnetCareDbContext : IdentityDbContext<EnetCareUser>, IEnetCareDbContext
    {
        public EnetCareDbContext()
            : base("name=EnetCareImsDatabase")
        {
            Configuration.LazyLoadingEnabled = true;
        }

        public EnetCareDbContext(string connection)
            : base(connection)
        {
            Configuration.LazyLoadingEnabled = true;
        }

        // Fields are virtual so that they may be overridden by mocks
        public virtual DbSet<Intervention> Interventions { get; set; }
        public virtual DbSet<InterventionType> InterventionTypes { get; set; }
        public virtual DbSet<InterventionApproval> InterventionApprovals { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<District> Districts { get; set; }

        public virtual IQueryable<Intervention> FullyLoadedInterventions
        {
            get
            {
                return Interventions
                  .Include(i => i.InterventionType)
                  .Include(i => i.Client)
                  .Include(i => i.Client.District)
                  .Include(i => i.SiteEngineer)
                  .Include(i => i.SiteEngineer.District)
                  .Include(i => i.Approval)
                  .Include(i => i.Approval.ApprovingManager)
                  .Include(i => i.Approval.ApprovingSiteEngineer)
                  .Include(i => i.Quality);
            }
        }

        public virtual IQueryable<Client> FullyLoadedClients
        {
            get
            {
                return Clients
                    .Include(m => m.District);
            }
        }

        /// <summary>
        /// Required for ASP Identity delegate
        /// </summary>
        public static EnetCareDbContext Create()
        {
            return new EnetCareDbContext();
        }
    }
}