using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ENETCare.IMS;
using ENETCare.IMS.Data.DataAccess;
using ENETCare.IMS.Interventions;

using ENETCare.IMS.WebApp.Models;

namespace ENETCare_IMS_WebApp.Controllers
{
    public class SiteEngineerController : Controller
    {
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
                    ActionName = "Index",
                },

                new NavbarItems.NavbarItem()
                {
                    Title = "Interventions",
                    BootstrapIcon = "fa-table",
                    ActionName = "Interventions",
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
                    ActionName = "Clients",
                },

                new NavbarItems.NavbarItem()
                {
                    Title = "Create a Client",
                    BootstrapIcon = "fa-user",
                    ActionName = "CreateNewClient",
                }
            );

            ViewBag.NavbarItems = items;
        }

        // GET: SiteEngineer
        public ActionResult Index()
        {
            SetNavbarItems();

            string accountType = "Site Engineer";
            ViewData["Title"] = accountType;

            return View();
        }

        public ActionResult Interventions()
        {
            SetNavbarItems();

            // Retrieve Interventions
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                InterventionRepo repo = new InterventionRepo(db);
                Interventions interventions =
                    repo.GetAllInterventions();

                return View(interventions);
            }
        }

        public ActionResult CreateIntervention()
        {
            SetNavbarItems();

            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                InterventionRepo interventionRepo = new InterventionRepo(db);
                ClientRepo clientRepo = new ClientRepo(db);

                InterventionTypes interventionTypes =
                    interventionRepo.GetAllInterventionTypes();
                Clients clients =
                    clientRepo.GetAllClients();

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
            if (ModelState.IsValid)
                return RedirectToAction("Interventions");

            SetNavbarItems();
            return CreateIntervention();
        }

        public ActionResult Clients()
        {
            SetNavbarItems();

            // Retrieve Clients
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                ClientRepo repo = new ClientRepo(db);
                Clients clients = repo.GetAllClients();  //To be replaced with GetClientsByDistrict(currentUser.District);

                return View(clients);
            }
        }

        public ActionResult CreateNewClient()
        {
            SetNavbarItems();

            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                DistrictRepo repo = new DistrictRepo(db);
                District district = repo.GetNthDistrict(1); //Replace with currentUser's District.DistrictID
                return View(new CreateNewClientViewModel()
                {
                    NewClientName = "",
                    NewDistrict = district,
                    NewLocationName = "",
                    NewDistrictID = district.DistrictID,
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewClient(CreateNewClientViewModel model)
        {
            // Check that the form validates
            if (!ModelState.IsValid)
                return CreateNewClient();

            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                DistrictRepo districtRepo = new DistrictRepo(db);
                District district = districtRepo.GetNthDistrict(model.NewDistrictID - 1);
                Client client = new Client(model.NewClientName, model.NewLocationName, district);
                ClientRepo clientRepo = new ClientRepo(db);
                clientRepo.Save(client);
            }

            return View("SaveNewClientComplete", model);
        }

        [HttpPost]
        public ActionResult SaveNewClientComplete(CreateNewClientViewModel model)
        {
            return View(model);
        }
    }
}