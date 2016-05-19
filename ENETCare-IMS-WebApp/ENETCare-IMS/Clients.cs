using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENETCare.IMS.Interventions;

namespace ENETCare.IMS
{
    public class Clients
    {
        private List<Client> clients;

        public Clients()
        {
            clients = new List<Client>();
        }

        /// <summary>
        /// Initializes a Clients DTO with a pre-defined collection of Clients.
        /// </summary>
        public Clients(List<Client> clients)
        {
            this.clients = clients;
        }

        public Client GetClientByID(int id)
        {
            if (id == 0)
                throw new IndexOutOfRangeException("ENETCare data is 1-indexed, but an index of 0 was requested.");
            return clients.First<Client>(c => c.ID == id);
        }

        public Client this[int index]
        {
            get
            {
                return clients[index];
            }
        }

        public Clients FilterByName(string name)
        {
            name = name.ToLower();
            var results =
                from client in clients
                where client.Name.ToLower().Contains(name)
                select client;
            return new Clients(results.ToList<Client>());
        }

        public Clients FilterByDistrict(District district)
        {
            var results =
                from client in clients
                where client.District == district
                select client;
            return new Clients(results.ToList<Client>());
        }

        public void Add(Client client)
        {
            clients.Add(client);
        }

        public Client CreateClient(string name, string location, District district)
        {
            Client newClient = new Client(name, location, district);
            Add(newClient);
            return newClient;
        }

        public List<Client> CopyAsList()
        {
            return new List<Client>(clients);
        }
    }
}
