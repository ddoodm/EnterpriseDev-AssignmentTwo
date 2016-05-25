using ENETCare.IMS.Data.DataAccess;
using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ENETCare_IMS_WebApp.Controllers
{
    public class ManagerController : Controller
    {
        string accountType = "Manager";

        // GET: Manager
        public ActionResult Index()
        {
            ViewData["Title"] = accountType;

            return View();
        }

        public ActionResult ViewProsposed()
        {
            ViewData["Title"] = accountType;

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
    }
}