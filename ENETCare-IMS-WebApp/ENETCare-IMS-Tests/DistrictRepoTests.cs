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
    public class DistrictRepoTests
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
            DbSet<District> districts = null)
        {
            var fakeDbContext = new Mock<EnetCareDbContext>("name=FakeDbContext");
            fakeDbContext.Setup(c => c.Interventions).Returns(interventions);
            fakeDbContext.Setup(c => c.Users).Returns(users);
            fakeDbContext.Setup(c => c.Districts).Returns(districts);

            // Override "fully loaded" Includer properties
            fakeDbContext.Setup(c => c.FullyLoadedInterventions).Returns(fakeDbContext.Object.Interventions);

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
        public void DistrictRepo_Get_Nth_Returns_Correct_District()
        {
            // Build fake district database
            var districts = new List<District>
            {
                new District(1, "A place"),
                new District(4, "B place"),
                new District(42, "C place"),
                new District(2941, "D place"),
            };
            var fakeDbSet = BuildMockDbSet<District>(districts);
            var fakeDbContext = BuildMockDbContext(districts: fakeDbSet);

            // Build repo from fake database
            DistrictRepo districtRepo = new DistrictRepo(fakeDbContext);

            // Test
            Assert.IsTrue(districtRepo.GetNthDistrict(0) == districts[0]);
            Assert.IsTrue(districtRepo.GetNthDistrict(1) == districts[1]);
            Assert.IsTrue(districtRepo.GetNthDistrict(2) == districts[2]);
            Assert.IsTrue(districtRepo.GetNthDistrict(3) == districts[3]);
        }
    }
}
