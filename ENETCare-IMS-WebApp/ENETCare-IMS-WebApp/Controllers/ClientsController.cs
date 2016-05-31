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

namespace ENETCare_IMS_WebApp.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        // GET: Clients
        public ActionResult Index()
        {
            string clientsTitle = "Clients";
            ViewData["Title"] = clientsTitle;

            SetNavbarItems();

            // Retrieve Clients
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                ClientRepo repo = new ClientRepo(db);
                Clients clients = repo.GetAllClients();  //To be replaced with GetClientsByDistrict(currentUser.District);

                return View(clients);
            }
        }

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
                   ControllerName = "Interventions",
                   BootstrapIcon = "fa-table",
                   ActionName = "Index",
               },

               new NavbarItems.NavbarItem()
               {
                   Title = "Create an Intervention",
                   BootstrapIcon = "fa-calendar",
                   ControllerName = "Interventions",
                   ActionName = "CreateIntervention",
               },

               new NavbarItems.NavbarItem()
               {
                   Title = "Clients",
                   BootstrapIcon = "fa-users",
                   ActionName = "Index",
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

        public ActionResult ViewClient(int ID)
        {
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                ClientRepo clientRepo = new ClientRepo(db);
                InterventionRepo interventionRepo = new InterventionRepo(db);

                Client client = clientRepo.GetClientById(ID);
                Interventions interventions = interventionRepo.GetInterventionHistory(client);

                return View(new ViewClientInterventionsViewModel()
                {
                    ClientName = client.DescriptiveName,
                    DistrictName = client.District.Name,
                    LocationName = client.Location,
                    Interventions = interventions,
                });
            }
        }

        public ActionResult CreateNewClient()
        {
            SetNavbarItems();

            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                UserRepo userRepo = new UserRepo(db);
                SiteEngineer engineer =
                    userRepo.GetUserById<SiteEngineer>(User.Identity.GetUserId());
                District district = engineer.District;

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
                District district = districtRepo.GetDistrictById(model.NewDistrictID);
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