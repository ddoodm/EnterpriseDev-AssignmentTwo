using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using ENETCare.IMS.Users;

namespace ENETCare.IMS
{
    public class District
    {
        [Key]
        public int DistrictID { get; private set; }

        [Required]
        public string Name { get; private set; }

        public ICollection<Client> Clients { get; private set; }
        public ICollection<EnetCareUser> Users { get; private set; }

        public District() { }

        public District(string name)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is District))
                return false;
            return ((District)obj).DistrictID == this.DistrictID;
        }

        public override int GetHashCode()
        {
            return this.DistrictID.GetHashCode();
        }

        public static bool operator ==(District lhs, District rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(District lhs, District rhs)
        {
            return !(lhs == rhs);
        }
    }
}
