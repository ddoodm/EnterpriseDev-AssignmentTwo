using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;

using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;
using System.Data.Entity.ModelConfiguration;

namespace ENETCare.IMS.Data.DataAccess
{
    public class GenericRepo<T>
    {
        protected EnetCareDbContext context;

        public GenericRepo(EnetCareDbContext context)
        {
            this.context = context;
        }
    }
}
