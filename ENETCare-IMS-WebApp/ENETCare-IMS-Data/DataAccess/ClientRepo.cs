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

        private IQueryable<Client> FullyLoadedClients
        {
            get
            {
                return context.Clients
                    .Include(m => m.District);
            }
        }

        public Clients GetAllClients()
        {
            return new Clients(FullyLoadedClients.ToList<Client>());
        }

        public Client GetNthClient(int n)
        {
            return FullyLoadedClients
                .OrderBy(c => c.ID).Skip(n)
                .FirstOrDefault<Client>();
        }

        public void Save(Client[] clients)
        {
            foreach (Client client in clients)
            {
                Save(client);
            }
        }

        public void Save(Client client)
        {
            context.Districts.Attach(client.District);
            context.Clients.Add(client);
           
            context.SaveChanges();
        }
    }
}
