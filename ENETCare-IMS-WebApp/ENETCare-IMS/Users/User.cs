using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS.Users
{
    public abstract class EnetCareUser : IEnetCareUser, IEquatable<EnetCareUser>
    {
        public int ID { get; private set; }
        public string Name { get; private set; }

        /// <summary>
        /// The User's position (title), ie "Site Engineer"
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// The page to which the User is directed upon log-in
        /// </summary>
        public abstract string HomePage { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is EnetCareUser))
                return false;

            return ((EnetCareUser)obj).ID == this.ID;
        }

        public bool Equals(EnetCareUser other)
        {
            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            return ID;
        }

        public static bool operator ==(EnetCareUser lhs, EnetCareUser rhs)
        {
            return lhs.ID == rhs.ID;
        }

        public static bool operator !=(EnetCareUser lhs, EnetCareUser rhs)
        {
            return !(lhs == rhs);
        }

        protected EnetCareUser(int ID, string name)
        {
            this.ID = ID;
            this.Name = name;
        }
    }
}
