using System.Collections.Generic;
using ENETCare.IMS.Users;

namespace ENETCare.IMS
{
    public interface IDistrict
    {
        ICollection<Client> Clients { get; }
        int DistrictID { get; }
        string Name { get; }
        ICollection<ILocalizedUser> Users { get; }

        string ToString();
    }
}