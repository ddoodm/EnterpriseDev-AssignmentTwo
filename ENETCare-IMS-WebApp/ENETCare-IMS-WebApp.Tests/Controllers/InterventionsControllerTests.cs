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

        private Mock<SiteEngineer> BuildMockEngineer(string name, string email, District district, decimal labour, decimal cost)
        {
            var fakeUser = new Mock<SiteEngineer>(name, email, "1234TestPass!", district, labour, cost);
            string fakeUserId = Guid.NewGuid().ToString("N");
            fakeUser.Setup(u => u.Id).Returns(fakeUserId);
            fakeUser.Setup(u => u.District).Returns(district);

            return fakeUser;
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
        public void InterventionsController_List_ShowOneRelevant_Success()
        {
            var fakeDistrict = new District("Test Place");
            var fakeClient = new Client("Test Client", "Test Location", fakeDistrict);

            // Configure a fake user set
            var fakeEngineer = BuildMockEngineer("Test Engineer", "test@enet.com", fakeDistrict, 20.0m, 2000m);
            var fakeUsers = new List<EnetCareUser>
            {
                fakeEngineer.Object
            };

            // Configure a fake DBSet which returns the fake list
            var fakeUserSet = BuildMockDbSet<EnetCareUser>(fakeUsers);

            // Configure known fake interventions
            var fakeInterventions = new List<Intervention>
            {
                Intervention.Factory.CreateIntervention(new InterventionType("Test Type", 400m, 10m), fakeClient, fakeEngineer.Object)
            };

            // Configure a fake DBSet which returns the fake list
            var fakeInterventionSet = BuildMockDbSet<Intervention>(fakeInterventions);

            // Configure a fake database which will return a known set of Interventions
            var fakeDbContext = BuildMockDbContext(
                interventions: fakeInterventionSet,
                users: fakeUserSet);

            // Set up a fake HTTP Context (makes an Identity user from the ENETCare user)
            var fakeHttp = BuildMockHttpContext(fakeEngineer.Object);

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
    }
}
