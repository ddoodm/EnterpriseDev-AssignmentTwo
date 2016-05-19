using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using ENETCare.IMS.Users;

namespace ENETCare.IMS
{
    public class District : IEquatable<District>
    {
        [Key]
        public int DistrictID { get; private set; }

        [Required]
        public string Name { get; private set; }

        public virtual ICollection<Client> Clients { get; private set; }
        public virtual ICollection<ILocalizedUser> Users { get; private set; }

        public District()
        {
            Clients = new List<Client>();
            Users = new List<ILocalizedUser>();
        }

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
            return this.Equals(obj as District);
        }

        public bool Equals(District other)
        {
            return this.DistrictID == other.DistrictID;
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
            return !lhs.Equals(rhs);
        }
    }
}
