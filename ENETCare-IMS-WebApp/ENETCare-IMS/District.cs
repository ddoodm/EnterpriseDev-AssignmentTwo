using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS
{
    public class District : IEquatable<District>
    {
        public int ID      { get; private set; }
        public string Name { get; private set; }

        public District(int id, string name)
        {
            this.ID = id;
            this.Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(District))
                return false;

            return ((District)obj).ID == this.ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public bool Equals(District other)
        {
            return other.ID == this.ID;
        }

        public static bool operator ==(District lhs, District rhs)
        {
            return lhs.ID == rhs.ID;
        }

        public static bool operator !=(District lhs, District rhs)
        {
            return !(lhs == rhs);
        }
    }
}
