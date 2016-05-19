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
            Console.WriteLine(">>>>\tBuilding ENETCare DB Context ...");

            using (var context = new EnetCareDbContext())
                PopulateInitialData(context);

            return 0;
        }

        private static void PopulateInitialData(EnetCareDbContext context)
        {
            DistrictRepo districts = new DistrictRepo(context);
            ClientRepo clients = new ClientRepo(context);
            UserRepo users = new UserRepo(context);
            InterventionRepo interventions = new InterventionRepo(context);

            // Clear the database (order is important)
            Console.WriteLine(">>>>\tErasing existing data ...");
            interventions.EraseAllInterventions();
            interventions.EraseAllInterventionTypes();
            users.EraseAllData();
            clients.EraseAllData();
            districts.EraseAllData();

            // Re-populate the database
            Console.WriteLine(">>>>\tPopulating data ...");
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

            EnetCareUser[] users = new EnetCareUser[]
            {
                new SiteEngineer("Deinyon Davies",  districtRepo.GetNthDistrict(0),  8,   1000),
                new SiteEngineer("Henry Saal",      districtRepo.GetNthDistrict(1),  10,  2000),
                new SiteEngineer("Hans Samson",     districtRepo.GetNthDistrict(2),  100, 10000),
                new SiteEngineer("Bob James",       districtRepo.GetNthDistrict(3),  10,  2000),
                new SiteEngineer("Takeshi Itoh",    districtRepo.GetNthDistrict(4),  5, 100),

                new Manager("Daum Park", districtRepo.GetNthDistrict(0), 100, 10000),

                new Accountant("Yiannis Chambers"),
            };

            userRepo.Save(users);
        }

        private static void PopulateClients(ClientRepo clientRepo, DistrictRepo districtRepo)
        {
            Console.WriteLine("Populating clients ...");

            Client[] clients = new Client[]
            {
                new Client("Jane Doe", "1 Fakeway Ln., Fakeville", districtRepo.GetNthDistrict(0)),
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

            Intervention[] interventions = new Intervention[]
            {
                Intervention.Factory.CreateIntervention(
                    interventionRepo.GetNthInterventionType(0),
                    clientRepo.GetNthClient(0),
                    userRepo.GetNthSiteEngineer(0)),

                Intervention.Factory.CreateIntervention(
                    interventionRepo.GetNthInterventionType(1),
                    clientRepo.GetNthClient(1),
                    userRepo.GetNthSiteEngineer(1)),

                Intervention.Factory.CreateIntervention(
                    interventionRepo.GetNthInterventionType(2),
                    clientRepo.GetNthClient(2),
                    userRepo.GetNthSiteEngineer(2)),

                Intervention.Factory.CreateIntervention(
                    interventionRepo.GetNthInterventionType(3),
                    clientRepo.GetNthClient(3),
                    userRepo.GetNthSiteEngineer(3)),

                Intervention.Factory.CreateIntervention(
                    interventionRepo.GetNthInterventionType(4),
                    clientRepo.GetNthClient(4),
                    userRepo.GetNthSiteEngineer(4)),
            };

            interventionRepo.Save(interventions);
        }
    }
}
