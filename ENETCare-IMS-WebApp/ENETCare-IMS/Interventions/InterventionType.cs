using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS.Interventions
{
    public struct InterventionType
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public decimal Cost { get; private set; }
        public decimal Labour { get; private set; }

        public InterventionType(int ID, string Name, decimal Cost, decimal Labour)
        {
            this.ID = ID;
            this.Name = Name;
            this.Cost = Cost;
            this.Labour = Labour;
        }
    }
}
