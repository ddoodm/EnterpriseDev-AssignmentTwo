using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ENETCare.IMS.Data.DataAccess;
using ENETCare.IMS.Interventions;

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
            return View();
        }
    }
}