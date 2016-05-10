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
        private ENETCareDAO application;

        private List<Client> clients;

        public Clients(ENETCareDAO application)
        {
            this.application = application;
            clients = new List<Client>();
        }

        /// <summary>
        /// Initializes a Clients DTO with a pre-defined collection of Clients.
        /// </summary>
        private Clients(ENETCareDAO application, List<Client> clients)
        {
            this.application = application;
            this.clients = clients;
        }

        public Client GetClientByID(int id)
        {
            if (id == 0)
                throw new IndexOutOfRangeException("ENETCare data is 1-indexed, but an index of 0 was requested.");
            return clients.First<Client>(c => c.ID == id);
        }

        public Clients FilterByName(string name)
        {
            name = name.ToLower();
            var results =
                from client in clients
                where client.Name.ToLower().Contains(name)
                select client;
            return new Clients(application, results.ToList<Client>());
        }

        public Clients FilterByDistrict(District district)
        {
            var results =
                from client in clients
                where client.District == district
                select client;
            return new Clients(application, results.ToList<Client>());
        }

        /// <summary>
        /// Computes the next available ID number
        /// </summary>
        private int NextID
        {
            get
            {
                if (clients.Count < 1)
                    return 0;

                var highestClient
                    = clients.OrderByDescending(i => i.ID)
                    .FirstOrDefault();
                return highestClient.ID + 1;
            }
        }

        public void Add(Client client)
        {
            clients.Add(client);
        }

        public Client CreateClient(string name, string location, District district)
        {
            int id = NextID;
            Client newClient = new Client(id, name, location, district);
            application.Save(newClient);
            Add(newClient);
            return newClient;
        }

        public List<Client> CopyAsList()
        {
            return new List<Client>(clients);
        }
    }
}
