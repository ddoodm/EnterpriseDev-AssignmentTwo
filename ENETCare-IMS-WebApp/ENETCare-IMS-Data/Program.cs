using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using ENETCare.IMS.Data.DataAccess;

using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;

namespace ENETCare.IMS.Data
{
    class Program
    {
        static int Main(string[] args)
        {
            PopulateInitialData();

            return 0;
        }

        private static void PopulateInitialData()
        {
            DistrictRepo districts = DistrictRepo.New;
            ClientRepo clients = ClientRepo.New;
            UserRepo users = UserRepo.New;
            InterventionRepo interventions = InterventionRepo.New;

            Console.WriteLine(">>>>\tErasing existing data");
            interventions.EraseAllInterventions();
            interventions.EraseAllInterventionTypes();
            users.EraseAllData();
            clients.EraseAllData();
            districts.EraseAllData();

            Console.WriteLine(">>>>\tPopulating data");
            PopulateDistricts(districts);
            PopulateClients(clients, districts);
            PopulateUsers(users, districts);
            PopulateInterventionTypes(interventions);
            PopulateInterventions(interventions, users, clients, districts);

            Console.WriteLine(">>>>\tSuccess! Press RETURN to exit.");
            Console.ReadLine();
        }

        /// <summary>
        /// Builds and saves a set of default districts
        /// </summary>
        private static void PopulateDistricts(DistrictRepo repo)
        {
            Console.WriteLine("Populating districts ...");

            District[] defaultDistricts = new District[]
            {
                new District("Urban Indonesia"),
                new District("Rural Indonesia"),
                new District("Urban Papua New Guinea"),
                new District("Rural Papua New Guinea"),
                new District("Sydney"),
                new District("Rural New South Wales"),
            };

            // Clear and re-load
            repo.EraseAllData();
            repo.Save(defaultDistricts);
        }

        private static void PopulateInterventionTypes(InterventionRepo repo)
        {
            Console.WriteLine("Populating types ...");

            InterventionType[] types = new InterventionType[]
            {
                new InterventionType("Supply and Install Portable Toilet",              600,    2),
                new InterventionType("Hepatitis Avoidance Training",                    0,      3),
                new InterventionType("Supply and Install Storm-proof Home Kit",         5000,   8),
                new InterventionType("Supply Mosquito Net",                             25,     0),
                new InterventionType("Install Water Pump",                              1200,   80),
                new InterventionType("Supply High-Volume Water Filter and Train Users", 2000,   1),
                new InterventionType("Prepare Sewerage Trench",                         0,      50),
            };

            repo.Save(types);
        }

        private static void PopulateUsers(UserRepo userRepo, DistrictRepo districtRepo)
        {
            Console.WriteLine("Populating users ...");

            SiteEngineer testUser = new SiteEngineer(
                "Bob Ross", districtRepo.GetNthDistrict(0), 100, 10000);

            userRepo.Save(new EnetCareUser[] { testUser });
        }

        private static void PopulateClients(ClientRepo clientRepo, DistrictRepo districtRepo)
        {
            Console.WriteLine("Populating clients ...");

            Client[] clients = new Client[]
            {
                new Client("Jane Dow", "1 Fakeway Ln., Fakeville", districtRepo.GetNthDistrict(0)),
                new Client("John Doe", "123 Fake St., Sunnyland", districtRepo.GetNthDistrict(1)),
                new Client("Markus Samson", "42 Answer Ave., Earthland",districtRepo.GetNthDistrict(2)),
                new Client("Migi Hidari", "5 Leftlane St., Rightville", districtRepo.GetNthDistrict(3)),
                new Client("Hannah Persson", "88 Infinity Rd., Foreverland", districtRepo.GetNthDistrict(4)),
                new Client("Sarah Higgins", "52 Stick St., The Sticks", districtRepo.GetNthDistrict(5)),
            };

            clientRepo.Save(clients);
        }

        private static void PopulateInterventions(InterventionRepo interventionRepo, UserRepo userRepo, ClientRepo clientRepo, DistrictRepo districtRepo)
        {
            Console.WriteLine("Populating interventions ...");

            InterventionTypes types = interventionRepo.AllInterventionTypes;

            SiteEngineer testUser = userRepo.GetNthSiteEngineer(0);

            Client client = clientRepo.GetNthClient(0);

            Intervention[] interventions = new Intervention[]
            {
                Intervention.Factory.CreateIntervention(types[0], client, (SiteEngineer)testUser)
            };

            interventionRepo.EraseAllInterventions();
            interventionRepo.Save(interventions);
        }
    }
}
