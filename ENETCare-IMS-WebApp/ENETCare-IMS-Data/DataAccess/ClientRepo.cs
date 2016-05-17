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

        public Clients AllClients
        {
            get
            {
                // Eager-load the 'District' object
                using (var db = new EnetCareDbContext())
                {
                    return new Clients(
                        db.Clients
                        .Include(m => m.District)
                        .ToList<Client>());
                }
            }
        }

        public Client GetNthClient(int n)
        {
            using (var db = new EnetCareDbContext())
            {
                return db.Clients
                    .Include(m => m.District)
                    .OrderBy(c => c.ID).Skip(n).First<Client>();
            }
        }

        public void EraseAllData()
        {
            using (var db = new EnetCareDbContext())
            {
                if (db.Clients.Count() < 1) return;
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
