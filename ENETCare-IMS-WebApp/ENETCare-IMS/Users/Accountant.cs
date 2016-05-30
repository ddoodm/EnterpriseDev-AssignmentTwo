using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS.Users
{
    public class Accountant : EnetCareUser
    {
        public override string Title { get { return "Accountant"; } }
        public override string HomePageAction { get { return "Index"; } }
        public override string HomePageController { get { return "Accountant"; } }
        public override string Role { get { return "Accountant"; } }

        private Accountant() { }

        public Accountant(string name, string email, string password)
            : base (name, email, password)
        {
        }
    }
}
