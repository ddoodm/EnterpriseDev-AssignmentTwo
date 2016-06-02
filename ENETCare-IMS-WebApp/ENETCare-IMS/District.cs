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
    }
}
