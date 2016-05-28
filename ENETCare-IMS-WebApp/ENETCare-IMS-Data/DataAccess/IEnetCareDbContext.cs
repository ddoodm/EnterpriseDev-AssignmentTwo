using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;
using System.Data.Entity;

namespace ENETCare.IMS.Data.DataAccess
{
    public abstract class BaseEnetCareDbContext : DbContext
    {
        DbSet<Intervention> Interventions { get; set; }
        DbSet<InterventionType> InterventionTypes { get; set; }
        DbSet<InterventionApproval> InterventionApprovals { get; set; }
        DbSet<EnetCareUser> UserProfiles { get; set; }
        DbSet<Client> Clients { get; set; }
        DbSet<District> Districts { get; set; }
    }
}