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
using ENETCare.IMS.WebApp.Models;

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
            fakeUser.Setup(u => u.MaxApprovableCost).Returns(cost);
            fakeUser.Setup(u => u.MaxApprovableLabour).Returns(labour);

            return fakeUser.Object;
        }

        private Manager BuildMockManager(string name, string email, District district, decimal labour, decimal cost)
        {
            var fakeUser = new Mock<Manager>(name, email, "1234TestPass!", district, labour, cost);
            string fakeUserId = Guid.NewGuid().ToString("N");
            fakeUser.Setup(u => u.Id).Returns(fakeUserId);
            fakeUser.Setup(u => u.District).Returns(district);
            fakeUser.Setup(u => u.MaxApprovableCost).Returns(cost);
            fakeUser.Setup(u => u.MaxApprovableLabour).Returns(labour);

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
            var fakePrincipal = new GenericPrincipal(fakeIdentity, new string[]
            { (fakeEnetUser is SiteEngineer)? "SiteEngineer" : "Manager" });
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

        private void BuildTestWebRequest(
            out EnetCareDbContext fakeDbContext,
            out ControllerContext fakeHttpContext)
        {
            var district = new District("Test District");

            // Configure a fake user set (with a poor user who cannot approve)
            var engineer = BuildMockEngineer("Test Engineer", "test@enet.com", district, 1, 1);
            var fakeUsers = new List<EnetCareUser> { engineer };
            var fakeUserSet = BuildMockDbSet<EnetCareUser>(fakeUsers);

            var client = new Client("Test Client", "Test Location", district);
            var intervention = Intervention.Factory.CreateIntervention(
                new InterventionType("Example Intervention", 123, 456),
                client, engineer);
            var interventions = new List<Intervention> { intervention };
            var fakeInterventionSet = BuildMockDbSet<Intervention>(interventions);

            // Output
            fakeDbContext = BuildMockDbContext(
                interventions: fakeInterventionSet,
                users: fakeUserSet);
            fakeHttpContext = BuildMockHttpContext(engineer).Object;
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
        public void InterventionsController_Approval_Buttons_Shown_Correctly_For_Proposed_Intervention_Thant_Cannot_Be_Approved_By_Its_Engineer()
        {
            // === Configure ===

            var district = new District("Test District");

            // Configure a fake user set (with a poor user who cannot approve)
            var engineer = BuildMockEngineer("Test Engineer", "test@enet.com", district, 1, 1);
            var fakeUsers = new List<EnetCareUser> { engineer };
            var fakeUserSet = BuildMockDbSet<EnetCareUser>(fakeUsers);

            var client = new Client("Test Client", "Test Location", district);
            var intervention = Intervention.Factory.CreateIntervention(
                new InterventionType("Example Intervention", 123, 456),
                client, engineer);
            var interventions = new List<Intervention> { intervention };
            var fakeInterventionSet = BuildMockDbSet<Intervention>(interventions);

            // Output
            var fakeDbContext = BuildMockDbContext(
                interventions: fakeInterventionSet,
                users: fakeUserSet);
            var fakeHttpContext = BuildMockHttpContext(engineer).Object;

            // === Call ===

            var controller = new InterventionsController(fakeDbContext);
            controller.ControllerContext = fakeHttpContext;

            var result = controller.Edit(0) as ViewResult;
            var model = result.Model as EditInterventionViewModel;

            // === Test ===

            // The intervention is proposed, and the engineer cannot approve it,
            // so we should only display (cancel)
            Assert.IsTrue(model.CanCancel);
            Assert.IsFalse(model.CanApprove);
            Assert.IsFalse(model.CanComplete);

            // The context user is an engineer, who can modify quality
            Assert.IsTrue(model.CanModifyQuality);
        }

        [TestMethod]
        public void InterventionsController_Approval_Buttons_Shown_Correctly_For_Proposed_Intervention_That_Can_Be_Approved_By_Manager()
        {
            // === Configure ===

            var district = new District("Test District");

            // Build an engineer who cannot approve, and a manager who can
            var engineer = BuildMockEngineer("Test Engineer", "test@enet.com", district, 1, 1);
            var manager = BuildMockManager("Test Manager", "testm@enet.com", district, 9999, 9999);
            var fakeUsers = new List<EnetCareUser> { engineer, manager };
            var fakeUserSet = BuildMockDbSet<EnetCareUser>(fakeUsers);

            var client = new Client("Test Client", "Test Location", district);
            var intervention = Intervention.Factory.CreateIntervention(
                new InterventionType("Example Intervention", 123, 456),
                client, engineer);
            var interventions = new List<Intervention> { intervention };
            var fakeInterventionSet = BuildMockDbSet<Intervention>(interventions);

            // Output (use Manager as acting Identity user in this context)
            var fakeDbContext = BuildMockDbContext(
                interventions: fakeInterventionSet,
                users: fakeUserSet);
            var fakeHttpContext = BuildMockHttpContext(manager).Object;

            // === Call ===

            var controller = new InterventionsController(fakeDbContext);
            controller.ControllerContext = fakeHttpContext;

            var result = controller.Edit(0) as ViewResult;
            var model = result.Model as EditInterventionViewModel;

            // === Test ===

            // The intervention can be cancelled by the manager, or approved
            Assert.IsTrue(model.CanCancel);
            Assert.IsTrue(model.CanApprove);
            Assert.IsFalse(model.CanComplete);

            // The context user is a manager, who can not modify quality
            Assert.IsFalse(model.CanModifyQuality);
        }

        [TestMethod]
        public void InterventionsController_Approval_Buttons_Shown_Correctly_For_Approved_Intervention_For_SiteEngineer()
        {
            // === Configure ===

            var district = new District("Test District");

            // Configure a fake user set
            var engineer = BuildMockEngineer("Test Engineer", "test@enet.com", district, 999, 999);
            var fakeUsers = new List<EnetCareUser> { engineer };
            var fakeUserSet = BuildMockDbSet<EnetCareUser>(fakeUsers);

            var client = new Client("Test Client", "Test Location", district);
            var intervention = Intervention.Factory.CreateIntervention(
                new InterventionType("Example Intervention", 123, 456),
                client, engineer);
            var interventions = new List<Intervention> { intervention };
            var fakeInterventionSet = BuildMockDbSet<Intervention>(interventions);

            // Output
            var fakeDbContext = BuildMockDbContext(
                interventions: fakeInterventionSet,
                users: fakeUserSet);
            var fakeHttpContext = BuildMockHttpContext(engineer).Object;

            // === Call ===

            var controller = new InterventionsController(fakeDbContext);
            controller.ControllerContext = fakeHttpContext;

            var result = controller.Edit(0) as ViewResult;
            var model = result.Model as EditInterventionViewModel;

            // === Test ===

            // The intervention is approved, and the intervention can be completed or cancelled
            Assert.IsTrue(model.CanCancel);
            Assert.IsFalse(model.CanApprove);
            Assert.IsTrue(model.CanComplete);

            // The context user is an engineer, who can modify quality
            Assert.IsTrue(model.CanModifyQuality);
        }

        [TestMethod]
        public void InterventionsController_Approval_Buttons_Shown_Correctly_For_Intervention_Approved_By_Manager_Where_Engineer_Could_Not_Approve()
        {
            // === Configure ===

            var district = new District("Test District");

            // Build an engineer who cannot approve, and a manager who can
            var engineer = BuildMockEngineer("Test Engineer", "test@enet.com", district, 1, 1);
            var manager = BuildMockManager("Test Manager", "testm@enet.com", district, 9999, 9999);
            var fakeUsers = new List<EnetCareUser> { engineer, manager };
            var fakeUserSet = BuildMockDbSet<EnetCareUser>(fakeUsers);

            var client = new Client("Test Client", "Test Location", district);
            var intervention = Intervention.Factory.CreateIntervention(
                new InterventionType("Example Intervention", 123, 456),
                client, engineer);

            // Have the manager approve the intervention
            intervention.Approve(manager);

            var interventions = new List<Intervention> { intervention };
            var fakeInterventionSet = BuildMockDbSet<Intervention>(interventions);

            // Output (use Engineer as acting Identity user in this context)
            var fakeDbContext = BuildMockDbContext(
                interventions: fakeInterventionSet,
                users: fakeUserSet);
            var fakeHttpContext = BuildMockHttpContext(engineer).Object;

            // === Call ===

            var controller = new InterventionsController(fakeDbContext);
            controller.ControllerContext = fakeHttpContext;

            var result = controller.Edit(0) as ViewResult;
            var model = result.Model as EditInterventionViewModel;

            // === Test ===

            // The intervention is approved (by a manager),
            // the site engineer is viewing the intervention,
            // they may complete or cancel it regardless of their funds.
            Assert.IsTrue(model.CanCancel);
            Assert.IsFalse(model.CanApprove);
            Assert.IsTrue(model.CanComplete);

            // The context user is an engineer, who can modify quality
            Assert.IsTrue(model.CanModifyQuality);
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
    }
}
