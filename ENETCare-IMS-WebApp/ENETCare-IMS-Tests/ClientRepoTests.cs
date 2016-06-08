using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ENETCare.IMS.Data.DataAccess;
using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;
using System.IO;

namespace ENETCare.IMS.Tests
{
    [TestClass]
    public class ClientRepoTests
    {
        private DbSet<T> BuildMockDbSet<T>(List<T> collection) where T : class
        {
            var queryable = collection.AsQueryable();

            var fakeSet = new Mock<DbSet<T>>();
            fakeSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            fakeSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            fakeSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            fakeSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            return fakeSet.Object;
        }

        private EnetCareDbContext BuildMockDbContext(
            DbSet<Intervention> interventions = null,
            DbSet<EnetCareUser> users = null,
            DbSet<Client> clients = null,
            DbSet<District> districts = null)
        {
            var fakeDbContext = new Mock<EnetCareDbContext>("name=FakeDbContext");
            fakeDbContext.Setup(c => c.Interventions).Returns(interventions);
            fakeDbContext.Setup(c => c.Users).Returns(users);
            fakeDbContext.Setup(c => c.Districts).Returns(districts);
            fakeDbContext.Setup(c => c.Clients).Returns(clients);

            // Override "fully loaded" Includer properties
            fakeDbContext.Setup(c => c.FullyLoadedInterventions).Returns(fakeDbContext.Object.Interventions);
            fakeDbContext.Setup(c => c.FullyLoadedClients).Returns(fakeDbContext.Object.Clients);

            return fakeDbContext.Object;
        }

        [TestInitialize]
        public void SetupDataDirectory()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string path = Path.GetFullPath(Path.Combine(
                appDirectory, @"..\..\"));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
        }

        [TestMethod]
        public void ClientRepo_Get_By_District_Success()
        {
            // Build fake district database
            var districts = new List<District>
            {
                new District(1, "Alpha"),
                new District(4, "Beta"),
                new District(42, "Gamma"),
                new District(2941, "Delta"),
            };
            var districtDbSet = BuildMockDbSet<District>(districts);

            var clients = new List<Client>
            {
                new Client("Alora Alorison", "Test Location", districts[0]),
                new Client("Sue Williams", "Test Location", districts[0]),
                new Client("Bill Mason", "Test Location", districts[2]),
                new Client("Mark Barks", "Test Location", districts[1]),
                new Client("Sukki Suxton", "Test Location", districts[0]),
            };
            var clientDbSet = BuildMockDbSet<Client>(clients);

            var fakeDbContext = BuildMockDbContext(
                districts: districtDbSet,
                clients: clientDbSet);

            // Build repo from fake database
            ClientRepo clientRepo = new ClientRepo(fakeDbContext);

            // Test
            var clientsInDistrict0 = clientRepo.GetClientsInDistrict(districts[0]);

            // Check that all filtered clients are in the right district
            foreach (Client c in clientsInDistrict0)
                Assert.IsTrue(c.District == districts[0]);

            // Check that all unfiltered clients have been filtered correctly
            foreach (Client c in clients)
                if (c.District == districts[0])
                    Assert.IsTrue(clientsInDistrict0.Contains(c));
        }
    }
}
