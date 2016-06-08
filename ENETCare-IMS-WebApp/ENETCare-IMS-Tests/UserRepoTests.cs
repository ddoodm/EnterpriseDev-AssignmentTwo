using ENETCare.IMS.Data.DataAccess;
using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS.Tests
{
    [TestClass]
    public class UserRepoTests
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
        public void UserRepo_Get_User_By_Email_Success()
        {
            // Build fake district database
            var districts = new List<District> { new District(1, "Alpha Place") };
            var fakeDistrictDbSet = BuildMockDbSet<District>(districts);

            var users = new List<EnetCareUser>
            {
                new SiteEngineer("Bill Bobs", "bill@test.com", "TestPass123!", districts[0], 400, 100),
                new Accountant("Bob Makk", "bob@test.com", "TestPass123!"),
                new Manager("Yuri Kawaguchi", "yuri@test.com", "TestPass123!", districts[0], 400, 100),
                new SiteEngineer("Ami Nakazono", "ami@test.com", "TestPass123!", districts[0], 400, 100),
            };
            var fakeUserDbSet = BuildMockDbSet<EnetCareUser>(users);

            var fakeDbContext = BuildMockDbContext(
                users: fakeUserDbSet,
                districts: fakeDistrictDbSet);

            // Build repo from fake database
            UserRepo userRepo = new UserRepo(fakeDbContext);

            // Test *generic* cases (any EnetCareUser)
            Assert.IsTrue(userRepo.GetUserByEmail<EnetCareUser>("ami@test.com") == users[3]);

            // Test *concrete* cases (specific subtypes)
            Assert.IsTrue(userRepo.GetUserByEmail<SiteEngineer>("bill@test.com") == users[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserRepo_Get_User_By_Email_Using_Wrong_Subclass_Type_Failure()
        {
            // Build fake district database
            var districts = new List<District> { new District(1, "Alpha Place") };
            var fakeDistrictDbSet = BuildMockDbSet<District>(districts);

            var users = new List<EnetCareUser>
            {
                new SiteEngineer("Bill Bobs", "bill@test.com", "TestPass123!", districts[0], 400, 100),
                new Accountant("Bob Makk", "bob@test.com", "TestPass123!"),
                new Manager("Yuri Kawaguchi", "yuri@test.com", "TestPass123!", districts[0], 400, 100),
                new SiteEngineer("Ami Nakazono", "ami@test.com", "TestPass123!", districts[0], 400, 100),
            };
            var fakeUserDbSet = BuildMockDbSet<EnetCareUser>(users);

            var fakeDbContext = BuildMockDbContext(
                users: fakeUserDbSet,
                districts: fakeDistrictDbSet);

            // Build repo from fake database
            UserRepo userRepo = new UserRepo(fakeDbContext);

            // Try to get a SiteEngineer who is actually a Manager
            // (Will throw an InvalidOperationException)
            Assert.IsTrue(userRepo.GetUserByEmail<SiteEngineer>("yuri@test.com") == users[2]);
        }
    }
}
