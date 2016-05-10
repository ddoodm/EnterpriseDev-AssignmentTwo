using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

//using DbUp;

namespace ENETCare_IMS_Data
{
    class Program
    {
        static int Main(string[] args)
        {
            var connectionString =
                args.FirstOrDefault()
                ?? @"Server=(localDb)\MSSQLLocalDB; Database=C:\USERS\DDOODM\SOURCE\REPOS\ENTERPRISEDEV-ASSIGNMENTONE\ENETCARE-IMS\ENETCARE-IMS-WEBAPP\APP_DATA\ENETCAREIMS.MDF; Trusted_connection=true";

           /* var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();
            
            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
#if DEBUG
                Console.ReadLine();
#endif
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            */
            return 0;
        }
    }
}
