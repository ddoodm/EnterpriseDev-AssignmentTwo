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

        public Clients GetAllClients()
        {
            return new Clients(context.FullyLoadedClients.ToList<Client>());
        }

        public Client GetClientById(int ID)
        {
            return context.FullyLoadedClients.SingleOrDefault<Client>(c => c.ID == ID);
        }

        public Client GetNthClient(int n)
        {
            return context.FullyLoadedClients
                .OrderBy(c => c.ID).Skip(n)
                .FirstOrDefault<Client>();
        }

        public Clients GetClientsInDistrict(District district)
        {
            return new Clients(
                context.FullyLoadedClients
                .Where(c => c.District.DistrictID == district.DistrictID)
                .ToList<Client>());
        }

        public void Save(Client[] clients)
        {
            foreach (Client client in clients)
                Save(client);
        }

        public void Save(Client client)
        {
            context.Districts.Attach(client.District);
            context.Clients.Add(client);
           
            context.SaveChanges();
        }
    }
}
