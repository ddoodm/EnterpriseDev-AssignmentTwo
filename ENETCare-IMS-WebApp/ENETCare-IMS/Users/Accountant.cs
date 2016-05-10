using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS.Users
{
    public class Accountant : EnetCareUser
    {
        private const string
            TITLE = "Accountant",
            HOMEPAGE = "Accountants";

        public override string Title
        {
            get
            {
                return TITLE;
            }
        }

        public override string HomePage
        {
            get
            {
                return HOMEPAGE;
            }
        }

        public Accountant(
            int ID,
            string name)
            : base (ID, name)
        {

        }
        
    }
}
