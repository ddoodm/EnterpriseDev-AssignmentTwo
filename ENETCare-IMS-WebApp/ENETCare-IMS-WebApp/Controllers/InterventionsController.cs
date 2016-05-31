using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ENETCare.IMS;
using ENETCare.IMS.Data.DataAccess;
using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;

using ENETCare.IMS.WebApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ENETCare_IMS_WebApp.Controllers
{
    [Authorize]
    public class InterventionsController : Controller
    {
        private EnetCareDbContext DbContext { get; set; }
        private ApplicationUserManager UserManager { get; set; }

        public InterventionsController()
            :base()
        {
            DbContext = new EnetCareDbContext();
            UserManager = new ApplicationUserManager(new UserStore<EnetCareUser>(DbContext));
        }

        // GET: Interventions
        public ActionResult Index()
        {
            string interventionsTitle = "Interventions";
            ViewData["Title"] = interventionsTitle;

            //SetNavbarItems();

            // Retrieve Interventions
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                InterventionRepo repo = new InterventionRepo(db);
                Interventions interventions =
                    repo.GetAllInterventions();

                return View(interventions);
            }
        }

        /*
        /// <summary>
        /// TODO: This will be a User method
        /// </summary>
        private void SetNavbarItems()
        {
            NavbarItems items = new NavbarItems(
                new NavbarItems.NavbarItem()
                {
                    Title = "Dashboard",
                    BootstrapIcon = "fa-dashboard",
                    ControllerName = "Home",
                    ActionName = "Index",
                },

                new NavbarItems.NavbarItem()
                {
                    Title = "Interventions",
                    BootstrapIcon = "fa-table",
                    ActionName = "Index",
                },

                new NavbarItems.NavbarItem()
                {
                    Title = "Create an Intervention",
                    BootstrapIcon = "fa-calendar",
                    ActionName = "CreateIntervention",
                },

                new NavbarItems.NavbarItem()
                {
                    Title = "Clients",
                    BootstrapIcon = "fa-users",
                    ControllerName = "Clients",
                    ActionName = "Index",
                },

                new NavbarItems.NavbarItem()
                {
                    Title = "Create a Client",
                    BootstrapIcon = "fa-user",
                    ControllerName = "Clients",
                    ActionName = "CreateNewClient",
                }
            );

            ViewBag.NavbarItems = items;
        }

        */

        public ActionResult CreateIntervention()
        {
            //SetNavbarItems();

            using (var db = new EnetCareDbContext())
            {
                var interventionRepo = new InterventionRepo(db);
                var clientRepo = new ClientRepo(db);
                var userRepo = new UserRepo(db);

                SiteEngineer engineer =
                    userRepo.GetUserById<SiteEngineer>(User.Identity.GetUserId());

                InterventionTypes interventionTypes =
                    interventionRepo.GetAllInterventionTypes();
                Clients clients =
                    clientRepo.GetClientsInDistrict(engineer.District);

                return View(new CreateInterventionViewModel()
                {
                    Types = interventionTypes,
                    Clients = clients,
                    Date = DateTime.Now,
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateIntervention(CreateInterventionViewModel model)
        {
            // Display validation errors
            if (!ModelState.IsValid)
                return CreateIntervention();

            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                InterventionRepo interventions = new InterventionRepo(db);
                ClientRepo clients = new ClientRepo(db);
                UserRepo users = new UserRepo(db);

                InterventionType type = interventions.GetInterventionTypeById(model.SelectedTypeID);
                Client client = clients.GetClientById(model.SelectedClientID);

                // Obtain the current session's user from the database
                SiteEngineer siteEngineer =
                    users.GetUserById<SiteEngineer>(User.Identity.GetUserId());

                Intervention intervention = Intervention.Factory.CreateIntervention(
                    type, client, siteEngineer, model.Labour, model.Cost, model.Date);

                interventions.Save(intervention);
            }

            return RedirectToAction("Index");
        }

        public ActionResult ViewProposed()
        {
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                InterventionRepo repo = new InterventionRepo(db);
                Interventions interventions =
                    repo.GetAllInterventions();

                interventions.FilterByState(InterventionApprovalState.Proposed);

                return View(interventions);
            }
        }

        public ActionResult ViewApproved()
        {
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                InterventionRepo repo = new InterventionRepo(db);
                Interventions interventions =
                    repo.GetAllInterventions();

                interventions.FilterByState(InterventionApprovalState.Approved);

                return View(interventions);
            }
        }

        public ActionResult EditIntervention()
        {
            return View();
        }

        /// <summary>
        /// Returns Intervention Type details as a JSON object.
        /// Used in Create Intervention AJAX transaction.
        /// </summary>
        [HttpPost]
        public JsonResult InterventionType(int ID)
        {
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                InterventionRepo repo = new InterventionRepo(db);
                InterventionType type = repo.GetInterventionTypeById(ID);
                return Json(type);
            }
        }
    }
}