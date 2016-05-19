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
        public ClientRepo(EnetCareDbContext context)
            : base(context, context.Clients)
        { }

        public Client GetNthClient(int n)
        {
            return context.Clients
                .Include(m => m.District)
                .OrderBy(c => c.ID).Skip(n)
                .FirstOrDefault<Client>();
        }

        public void EraseAllData()
        {
            if (context.Clients.Count() < 1) return;
            context.Clients.RemoveRange(context.Clients);
            context.SaveChanges();
        }

        public void Save(Client[] clients)
        {
            foreach (Client client in clients)
            {
                context.Districts.Attach(client.District);
                context.Clients.Add(client);
            }

            context.SaveChanges();
        }
    }
}
