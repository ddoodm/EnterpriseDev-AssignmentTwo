using ENETCare.IMS.Interventions;
using System.Data.Entity;

namespace ENETCare.IMS.Data.DataAccess
{
    public interface IEnetCareDbContext
    {
        DbSet<Intervention> Interventions { get; set; }
        DbSet<InterventionType> InterventionTypes { get; set; }
        DbSet<InterventionApproval> InterventionApprovals { get; set; }
        DbSet<Client> Clients { get; set; }
        DbSet<District> Districts { get; set; }
    }
}