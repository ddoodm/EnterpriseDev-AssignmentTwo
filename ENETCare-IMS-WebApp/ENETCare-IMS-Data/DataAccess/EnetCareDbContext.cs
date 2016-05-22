using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;

using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;

namespace ENETCare.IMS.Data.DataAccess
{
    public class EnetCareDbContext : DbContext
    {
        public DbSet<Intervention> Interventions { get; set; }
        public DbSet<InterventionType> InterventionTypes { get; set; }

        public DbSet<EnetCareUser> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<District> Districts { get; set; }
    }
}