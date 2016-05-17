using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;

using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;
using System.Data.Entity.ModelConfiguration;

namespace ENETCare.IMS.Data.DataAccess
{
    public class GenericRepo<T>
    {
        protected class EnetCareDbContext : DbContext
        {
            public DbSet<Intervention>      Interventions { get; set; }
            public DbSet<InterventionType>  InterventionTypes { get; set; }

            public DbSet<Client>            Clients { get; set; }
            public DbSet<District>          Districts { get; set; }
            public DbSet<EnetCareUser>      Users { get; set; }

            public EnetCareDbContext() : base("EnetCareDbContext")
            { }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Entity<EnetCareUser>()
                    .HasRequired<District>(u => u.District)
                    .WithMany(d => d.Users);

                modelBuilder.Entity<Client>()
                    .HasRequired<District>(c => c.District)
                    .WithMany(d => d.Clients);

                base.OnModelCreating(modelBuilder);
            }
        }
    }
}
