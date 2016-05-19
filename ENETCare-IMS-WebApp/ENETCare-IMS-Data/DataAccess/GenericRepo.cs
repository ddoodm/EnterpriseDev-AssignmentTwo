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
    public abstract class GenericRepo<T> where T : class
    {
        protected EnetCareDbContext context;
        private DbSet<T> dataSource;

        protected GenericRepo(EnetCareDbContext context, DbSet<T> dataSource)
        {
            this.context = context;
            this.dataSource = dataSource;
        }

        /// <summary>
        /// The number of elements in this repository
        /// </summary>
        public int Count
        {
            get { return dataSource.Count(); }
        }
    }
}
