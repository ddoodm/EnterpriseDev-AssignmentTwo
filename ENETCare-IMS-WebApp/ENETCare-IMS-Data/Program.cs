using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

using ENETCare.IMS.Data.DataAccess;

using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;
using ENETCare.IMS.Data.Migrations;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Diagnostics;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace ENETCare.IMS.Data
{
    class Program
    {
        /// <summary>
        /// Sets the path of the Data directory for
        /// the Connection String to correctly attach
        /// the MDF database to the server.
        /// 
        /// This method may be called by the EF Migrator,
        /// and as such, may originate from either project directory.
        /// 
        /// This method supports only origins "Data" and "WebApp"
        /// </summary>
        public static void SetupDataDirectory()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string immediateDirectoryName = Path.GetFileName(Path.GetDirectoryName(appDirectory));

            // Determine the relative path of the Data directory
            string relativePath = "";
            switch(immediateDirectoryName)
            {
                case "ENETCare-IMS-Data":   relativePath = @"..\..\"; break;
                case "ENETCare-IMS-WebApp": relativePath = @"..\ENETCare-IMS-Data\"; break;
                default:
                    Debug.WriteLine(
                        "Assuming relative path to Data Directory is '..\\..\\..\\ENETCare-IMS-Data'.\nCannot determine the relative path of the Data Directory from the application path:\n"
                        + appDirectory + "\n(" + immediateDirectoryName + ")");
                    relativePath = @"..\..\..\ENETCare-IMS-Data";
                    break;
            }

            string path = Path.GetFullPath(Path.Combine(
                appDirectory, relativePath));

            AppDomain.CurrentDomain.SetData("DataDirectory", path);
        }

        public static void Main()
        {
            SetupDataDirectory();

            Console.WriteLine("Building a DB Context ...");

            using (var db = new EnetCareDbContext())
                SeedDatabase(db);
        }

        public static void SeedDatabase(EnetCareDbContext context)
        {
            DistrictRepo districts = new DistrictRepo(context);
            ClientRepo clients = new ClientRepo(context);
            UserRepo users = new UserRepo(context);
            InterventionRepo interventions = new InterventionRepo(context);

            // Clear the database (order is important)
            Console.WriteLine(">>>>\tErasing existing data ...");
            interventions.EraseAllData();
            users.EraseAllData();
            clients.EraseAllData();
            districts.EraseAllData();

            // Re-populate the database
            Console.WriteLine(">>>>\tPopulating data ...");
            PopulateDistricts(districts);
            PopulateClients(clients, districts);
            PopulateUsers(users, districts, context);
            PopulateInterventionTypes(interventions);
            PopulateInterventions(interventions, users, clients, districts);
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

        private static void PopulateUsers(UserRepo userRepo, DistrictRepo districtRepo, EnetCareDbContext context)
        {
            Console.WriteLine("Populating users ...");

            EnetCareUser[] users = new EnetCareUser[]
            {
                new SiteEngineer("Deinyon Davies",  "deinyond@gmail.com", "TestPass1!", districtRepo.GetNthDistrict(0),  8,   1000),
                new SiteEngineer("Henry Saal",      "henry@enet.com", "TestPass1!",     districtRepo.GetNthDistrict(1),  10,  2000),
                new SiteEngineer("Hans Samson",     "hans@enet.com", "TestPass1!",      districtRepo.GetNthDistrict(2),  100, 10000),
                new SiteEngineer("Bob James",       "bob@enet.com", "TestPass1!",       districtRepo.GetNthDistrict(3),  10,  2000),
                new SiteEngineer("Takeshi Itoh",    "takeshi@enet.com", "TestPass1!",   districtRepo.GetNthDistrict(4),  5, 100),

                new Manager("Daum Park", "daum@enet.com", "TestPass1!", districtRepo.GetNthDistrict(0), 100, 10000),

                new Accountant("Yiannis Chambers", "yiannis@enet.com", "TestPass1!"),
            };

            userRepo.Save(users);

            // Build user roles from users
            PopulateRoles(context, users);
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
                    userRepo.GetUserByEmail<SiteEngineer>("deinyond@gmail.com")),

                Intervention.Factory.CreateIntervention(
                    interventionRepo.GetNthInterventionType(1),
                    clientRepo.GetNthClient(1),
                    userRepo.GetUserByEmail<SiteEngineer>("henry@enet.com")),

                Intervention.Factory.CreateIntervention(
                    interventionRepo.GetNthInterventionType(2),
                    clientRepo.GetNthClient(2),
                    userRepo.GetUserByEmail<SiteEngineer>("hans@enet.com")),

                Intervention.Factory.CreateIntervention(
                    interventionRepo.GetNthInterventionType(3),
                    clientRepo.GetNthClient(3),
                    userRepo.GetUserByEmail<SiteEngineer>("bob@enet.com")),

                Intervention.Factory.CreateIntervention(
                    interventionRepo.GetNthInterventionType(4),
                    clientRepo.GetNthClient(4),
                    userRepo.GetUserByEmail<SiteEngineer>("takeshi@enet.com")),
            };

            interventions[0].UpdateNotes(interventions[0].SiteEngineer, "Test notes");

            interventionRepo.Save(interventions);
        }

        private static void PopulateRoles(EnetCareDbContext context, EnetCareUser[] users)
        {
            Console.WriteLine(">>>>\tBuilding user roles ...");

            // TODO: Should use ENETCareRole / store / manager, but they are in the Web assembly
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            // TODO: Should use Application user store / manager, but they are in the Web assembly
            var userStore = new UserStore<EnetCareUser>(context);
            var userManager = new UserManager<EnetCareUser>(userStore);

            // For each user, add their role if it does not already exist
            foreach (EnetCareUser user in users)
            {
                // Create new roles
                if (!roleManager.RoleExists(user.Role))
                    roleManager.Create(new IdentityRole(user.Role));

                // Add user to role
                userManager.AddToRole(user.Id, user.Role);
            }
        }
    }
}
