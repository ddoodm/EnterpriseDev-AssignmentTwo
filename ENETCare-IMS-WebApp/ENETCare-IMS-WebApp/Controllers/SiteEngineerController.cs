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
        // GET: SiteEngineer
        public ActionResult Index()
        {
            string accountType = "Site Engineer";
            ViewData["Title"] = accountType;

            return View();
        }

        public ActionResult Interventions()
        {
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

        public ActionResult Clients()
        {
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
        public ActionResult SaveNewClientComplete(CreateNewClientViewModel model)
        {

            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                DistrictRepo districtRepo = new DistrictRepo(db);
                District district = districtRepo.GetNthDistrict(model.NewDistrictID - 1);
                Client client = new Client(model.NewClientName, model.NewLocationName, district);
                ClientRepo clientRepo = new ClientRepo(db);
                clientRepo.Save(client);
            }

            return View(model);
            
        }

    }
}