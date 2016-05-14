using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using ENETCare.IMS.Data.DataAccess;

using ENETCare.IMS.Interventions;

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
            PopulateDistricts();
            PopulateInterventionTypes();
        }

        /// <summary>
        /// Builds and saves a set of default districts
        /// </summary>
        private static void PopulateDistricts()
        {
            DistrictRepo repo = DistrictRepo.New;

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

        private static void PopulateInterventionTypes()
        {
            InterventionRepo repo = InterventionRepo.New;

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

            // Clear and re-load
            repo.EraseAllInterventionTypes();
            repo.Save(types);
        }
    }
}
