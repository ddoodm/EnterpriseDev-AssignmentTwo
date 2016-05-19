using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace ENETCare.IMS.Interventions
{
    public class InterventionType
    {
        [Key]
        public int ID { get; private set; }

        [Required]
        public string Name { get; private set; }

        [Required]
        public decimal Cost { get; private set; }

        [Required]
        public decimal Labour { get; private set; }

        public InterventionType() { }

        public InterventionType(string Name, decimal Cost, decimal Labour)
        {
            this.Name = Name;
            this.Cost = Cost;
            this.Labour = Labour;
        }
    }
}
