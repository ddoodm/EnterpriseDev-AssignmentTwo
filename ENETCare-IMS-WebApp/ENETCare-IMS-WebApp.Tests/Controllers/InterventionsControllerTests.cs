using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ENETCare.IMS.Data.DataAccess;

using Moq;
using System.Data.Entity;
using ENETCare.IMS.Interventions;
using ENETCare_IMS_WebApp.Controllers;
using System.Web.Mvc;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.Routing;
using ENETCare.IMS.Users;
using ENETCare.IMS;
using System.Threading;
using System.Security.Claims;
using System.Linq.Expressions;

namespace ENETCare_IMS_WebApp.Tests.Controllers
{
    [TestClass]
    public class InterventionsControllerTests
    {
        #region Mock helper functions

        private SiteEngineer BuildMockEngineer(string name, string email, District district, decimal labour, decimal cost)
        {
            var fakeUser = new Mock<SiteEngineer>(name, email, "1234TestPass!", district, labour, cost);
            string fakeUserId = Guid.NewGuid().ToString("N");
            fakeUser.Setup(u => u.Id).Returns(fakeUserId);
            fakeUser.Setup(u => u.District).Returns(district);

            return fakeUser.Object;
        }

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

        private GenericPrincipal BuildMockIdentityUser(EnetCareUser fakeEnetUser)
        {
            var fakeIdentity = new GenericIdentity("Test Engineer");

            // Configure user ID:
            fakeIdentity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", fakeEnetUser.Id));
            var fakePrincipal = new GenericPrincipal(fakeIdentity, new string[] { "SiteEngineer" });
            Thread.CurrentPrincipal = fakePrincipal;

            return fakePrincipal;
        }

        private EnetCareDbContext BuildMockDbContext(
            DbSet<Intervention> interventions = null,
            DbSet<EnetCareUser> users = null)
        {
            var fakeDbContext = new Mock<EnetCareDbContext>();
            fakeDbContext.Setup(c => c.Interventions).Returns(interventions);
            fakeDbContext.Setup(c => c.Users).Returns(users);

            // Override "fully loaded" Includer properties
            fakeDbContext.Setup(c => c.FullyLoadedInterventions).Returns(fakeDbContext.Object.Interventions);

            return fakeDbContext.Object;
        }

        private Mock<ControllerContext> BuildMockHttpContext(EnetCareUser fakeUser)
        {
            // Set up the fake Identity user
            var fakePrincipal = BuildMockIdentityUser(fakeUser);

            var fakeHttp = new Mock<ControllerContext>();
            fakeHttp.SetupGet(h => h.HttpContext.User).Returns(fakePrincipal);
            fakeHttp.SetupGet(h => h.HttpContext.Request.IsAuthenticated).Returns(true);

            return fakeHttp;
        }

        #endregion

        [TestInitialize]
        public void SetupDataDirectory()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string path = Path.GetFullPath(Path.Combine(
                appDirectory, @"..\..\"));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
        }

        [TestMethod]
        public void InterventionsController_List_Shows_One_Relevant_Success()
        {
            var fakeDistrict = new District("Test Place");
            var fakeClient = new Client("Test Client", "Test Location", fakeDistrict);

            // Configure a fake user set
            var fakeEngineer = BuildMockEngineer("Test Engineer", "test@enet.com", fakeDistrict, 20.0m, 2000m);
            var fakeUsers = new List<EnetCareUser>
            {
                fakeEngineer
            };

            // Configure a fake DBSet which returns the fake list
            var fakeUserSet = BuildMockDbSet<EnetCareUser>(fakeUsers);

            // Configure known fake interventions
            var fakeInterventions = new List<Intervention>
            {
                Intervention.Factory.CreateIntervention(new InterventionType("Test Type", 400m, 10m), fakeClient, fakeEngineer)
            };

            // Configure a fake DBSet which returns the fake list
            var fakeInterventionSet = BuildMockDbSet<Intervention>(fakeInterventions);

            // Configure a fake database which will return a known set of Interventions
            var fakeDbContext = BuildMockDbContext(
                interventions: fakeInterventionSet,
                users: fakeUserSet);

            // Set up a fake HTTP Context (makes an Identity user from the ENETCare user)
            var fakeHttp = BuildMockHttpContext(fakeEngineer);

            // Call to the controller
            var controller = new InterventionsController(fakeDbContext);
            controller.ControllerContext = fakeHttp.Object;

            // Call the action (no 'state' parameter)
            var result = controller.Index(null) as ViewResult;

            // Verify output (model is Interventions collection)
            var model = (Interventions)result.Model;

            if (model.Count != fakeInterventions.Count)
                Assert.Fail("The model is not equal to the test interventions list");

            for(int i = 0; i < model.Count; i++)
                if(model[i] != fakeInterventions[i])
                    Assert.Fail("The model is not equal to the test interventions list");
        }

        [TestMethod]
        public void InterventionController_List_Shows_And_Culls_Success()
        {
            var fakeDistricts = new District[]
            {
                new District(0, "Alpha Place"),
                new District(1, "Beta Place"),
                new District(2, "Gamma Place"),
            };
            var fakeClients = new Client[]
            {
                new Client("A A Client", "Test Location", fakeDistricts[0]),
                new Client("A B Client", "Test Location", fakeDistricts[0]),
                new Client("B C Client", "Test Location", fakeDistricts[1]),
                new Client("C D Client", "Test Location", fakeDistricts[2]),
            };

            // Configure a fake user set
            var fakeEngineers = new SiteEngineer[]
            {
                BuildMockEngineer("A A Engineer", "aa@enet.com", fakeDistricts[0], 20.0m, 2000m),
                BuildMockEngineer("A B Engineer", "ab@enet.com", fakeDistricts[0], 20.0m, 2000m),
                BuildMockEngineer("B C Engineer", "bc@enet.com", fakeDistricts[1], 20.0m, 2000m),
            };

            // Configure a fake DBSet which returns the fake list
            var fakeUserSet = BuildMockDbSet<EnetCareUser>(fakeEngineers.ToList<EnetCareUser>());

            // Configure known fake interventions
            var fakeInterventions = new List<Intervention>
            {
                Intervention.Factory.CreateIntervention(new InterventionType("Type AAA", 400m, 10m), fakeClients[0], fakeEngineers[0]),
                Intervention.Factory.CreateIntervention(new InterventionType("Type AAB", 400m, 10m), fakeClients[1], fakeEngineers[0]),
                Intervention.Factory.CreateIntervention(new InterventionType("Type AAC", 400m, 10m), fakeClients[0], fakeEngineers[1]),
                Intervention.Factory.CreateIntervention(new InterventionType("Type AAD", 400m, 10m), fakeClients[2], fakeEngineers[2]),
            };

            // Configure a fake DBSet which returns the fake list
            var fakeInterventionSet = BuildMockDbSet<Intervention>(fakeInterventions);

            // Configure a fake database which will return a known set of Interventions
            var fakeDbContext = BuildMockDbContext(
                interventions: fakeInterventionSet,
                users: fakeUserSet);

            // Set up a fake HTTP Context (makes an Identity user from the ENETCare user)
            var fakeHttp = BuildMockHttpContext(fakeEngineers[0]);

            // Call to the controller
            var controller = new InterventionsController(fakeDbContext);
            controller.ControllerContext = fakeHttp.Object;

            // Call the action (no 'state' parameter)
            var result = controller.Index(null) as ViewResult;

            // Verify output (model is Interventions collection)
            var model = (Interventions)result.Model;

            // TODO: Figure out why interventions are not filtered in this test
            Assert.Fail("The interventions are not filtered, but it works on the Web App, so this test is broken.");
        }
    }
}
