using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;

namespace ENETCare.IMS.Data.DataAccess
{
    public class ClientRepo : GenericRepo<Client>
    {
        public static ClientRepo New
        {
            get { return new ClientRepo(); }
        }
    }
}
