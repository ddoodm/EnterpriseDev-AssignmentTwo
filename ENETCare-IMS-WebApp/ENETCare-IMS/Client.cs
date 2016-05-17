using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ENETCare.IMS.Interventions;

namespace ENETCare.IMS
{
    public class Client
    {
        [Key]
        public int ID                       { get; private set; }

        [Required]
        public string Name                  { get; private set; }

        public int? DistrictID               { get; private set; }
        [ForeignKey("DistrictID")]
        public virtual District District    { get; private set; }

        [Required]
        public string Location              { get; private set; }

        public ICollection<Intervention> Interventions { get; private set; }

        public string DescriptiveName
        {
            get
            {
                string format = "{0} ({1}, {2})";
                return String.Format(format, Name, Location, District);
            }
        }

        public Client() { }

        public Client(string name, string location, District district)
        {
            this.Name = name;
            this.Location = location;
            this.District = district;
        }
    }
}
