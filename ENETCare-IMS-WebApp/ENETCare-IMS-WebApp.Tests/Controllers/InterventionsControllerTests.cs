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
            // Fake district
            var fakeDistrict = new District("Test Place");

            // Fake client
            var fakeClient = new Client("Test Client", "Test Location", fakeDistrict);

            // Configure a fake user set
            var fakeUser = new Mock<SiteEngineer>("Test Engineer", "test@enet.com", "1234TestPass!", fakeDistrict, 48m, 20000m);
            string fakeUserId = Guid.NewGuid().ToString("N");
            fakeUser.Setup(u => u.Id).Returns(fakeUserId);
            fakeUser.Setup(u => u.District).Returns(fakeDistrict);
            var fakeUsers = new List<EnetCareUser>
            {
                fakeUser.Object
            }.AsQueryable();

            // Configure a fake DBSet which returns the fake list
            var fakeUserSet = new Mock<DbSet<EnetCareUser>>();
            fakeUserSet.As<IQueryable<EnetCareUser>>().Setup(m => m.Provider).Returns(fakeUsers.Provider);
            fakeUserSet.As<IQueryable<EnetCareUser>>().Setup(m => m.Expression).Returns(fakeUsers.Expression);
            fakeUserSet.As<IQueryable<EnetCareUser>>().Setup(m => m.ElementType).Returns(fakeUsers.ElementType);
            fakeUserSet.As<IQueryable<EnetCareUser>>().Setup(m => m.GetEnumerator()).Returns(fakeUsers.GetEnumerator());

            // Configure known fake interventions
            var fakeInterventions = new List<Intervention>
            {
                Intervention.Factory.CreateIntervention(new InterventionType("Test Type", 400m, 10m), fakeClient, fakeUser.Object)
            };
            var fakeInterventionsQueriable = fakeInterventions.AsQueryable();

            // Configure a fake DBSet which returns the fake list
            var fakeInterventionSet = new Mock<DbSet<Intervention>>();
            fakeInterventionSet.As<IQueryable<Intervention>>().Setup(m => m.Provider).Returns(fakeInterventionsQueriable.Provider);
            fakeInterventionSet.As<IQueryable<Intervention>>().Setup(m => m.Expression).Returns(fakeInterventionsQueriable.Expression);
            fakeInterventionSet.As<IQueryable<Intervention>>().Setup(m => m.ElementType).Returns(fakeInterventionsQueriable.ElementType);
            fakeInterventionSet.As<IQueryable<Intervention>>().Setup(m => m.GetEnumerator()).Returns(fakeInterventionsQueriable.GetEnumerator());

            // Configure a fake database which will return a known set of Interventions
            var fakeContext = new Mock<EnetCareDbContext>();
            fakeContext.Setup(c => c.Interventions).Returns(fakeInterventionSet.Object);
            fakeContext.Setup(c => c.Users).Returns(fakeUserSet.Object);

            // Override "fully loaded" Includer properties
            fakeContext.Setup(c => c.FullyLoadedInterventions).Returns(fakeContext.Object.Interventions);

            // Set up a fake Identity user
            var fakeIdentity = new GenericIdentity("Test Engineer");
            // Configure user ID:
            fakeIdentity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", fakeUser.Object.Id));
            var fakePrincipal = new GenericPrincipal(fakeIdentity, new string[] { "SiteEngineer" });
            Thread.CurrentPrincipal = fakePrincipal;

            // Set up a fake HTTP Context
            var fakeHttp = new Mock<ControllerContext>();
            fakeHttp.SetupGet(h => h.HttpContext.User).Returns(fakePrincipal);
            fakeHttp.SetupGet(h => h.HttpContext.Request.IsAuthenticated).Returns(true);

            // Call to the controller
            var controller = new InterventionsController(fakeContext.Object);
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
