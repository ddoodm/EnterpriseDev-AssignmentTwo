namespace ENETCare.IMS.Data.Migrations
{
    using DataAccess;
    using Interventions;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Users;

    public sealed class Configuration : DbMigrationsConfiguration<ENETCare.IMS.Data.DataAccess.EnetCareDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

            Program.SetupDataDirectory();
        }

        /// <summary>
        /// This method will be called after migrating to the latest version.
        /// </summary>
        protected override void Seed(ENETCare.IMS.Data.DataAccess.EnetCareDbContext context)
        {
            Program.SeedDatabase(context);
        }
    }
}
