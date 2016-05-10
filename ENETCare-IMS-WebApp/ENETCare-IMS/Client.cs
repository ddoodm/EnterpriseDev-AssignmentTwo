using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS
{
    public class Client
    {
        public int ID               { get; private set; }
        public string Name          { get; private set; }
        public District District    { get; private set; }
        public string Location      { get; private set; }

        public string DescriptiveName
        {
            get
            {
                string format = "{0} ({1}, {2})";
                return String.Format(format, Name, Location, District);
            }
        }

        public Client(int ID, string name, string location, District district)
        {
            this.ID = ID;
            this.Name = name;
            this.Location = location;
            this.District = district;
        }
    }
}
