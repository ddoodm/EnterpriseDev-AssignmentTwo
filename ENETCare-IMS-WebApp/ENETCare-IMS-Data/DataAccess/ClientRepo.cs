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

        public void EraseAllData()
        {
            using (var db = new EnetCareDbContext())
            {
                db.Clients.RemoveRange(db.Clients);
                db.SaveChanges();
            }
        }

        public void Save(Client[] clients)
        {
            using (var db = new EnetCareDbContext())
            {
                foreach (Client client in clients)
                    db.Clients.Add(client);
                db.SaveChanges();
            }
        }
    }
}
