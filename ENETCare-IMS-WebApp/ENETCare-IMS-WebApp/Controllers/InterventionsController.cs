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
    [Authorize(Roles = "SiteEngineer, Manager")]
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
        [Authorize(Roles = "SiteEngineer")]
        public ActionResult Index()
        {
            string interventionsTitle = "Interventions";
            ViewData["Title"] = interventionsTitle;

            // Retrieve Interventions
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                InterventionRepo repo = new InterventionRepo(db);
                Interventions interventions =
                    repo.GetAllInterventions();

                return View(interventions);
            }
        }

        [Authorize(Roles = "SiteEngineer")]
        public ActionResult CreateIntervention()
        {
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
        [Authorize(Roles = "SiteEngineer")]
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

        [Authorize(Roles = "Manager")]
        public ActionResult ViewProposed()
        {
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                InterventionRepo repo = new InterventionRepo(db);
                Interventions interventions =
                    repo.GetAllInterventions();

                interventions =
                    interventions.FilterByState(InterventionApprovalState.Proposed);

                return View(interventions);
            }
        }

        [Authorize(Roles = "Manager")]
        public ActionResult ViewApproved()
        {
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                InterventionRepo repo = new InterventionRepo(db);
                Interventions interventions =
                    repo.GetAllInterventions();

                interventions =
                    interventions.FilterByState(InterventionApprovalState.Approved);

                return View(interventions);
            }
        }

        [Authorize(Roles = "SiteEngineer, Manager")]
        public ActionResult Edit(int ID)
        {
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                InterventionRepo interventionRepo = new InterventionRepo(db);
                Intervention intervention = interventionRepo.GetAllInterventions().GetInterventions().Where(i => i.ID == ID).First();
                var users = new UserRepo(db);
                var user = users.GetUserById<EnetCareUser>(User.Identity.GetUserId());

                bool canModifyState = intervention.UserCanChangeState((IInterventionApprover)user);
                bool canModifyQuality = intervention.UserCanChangeQuality(user);
                bool canApprove = intervention.CanApprove();
                bool canCancel = intervention.CanCancel();
                bool canComplete = intervention.CanComplete();

                EditInterventionViewModel model = new EditInterventionViewModel();
                model.Intervention = intervention;
                model.CanApprove = canApprove;
                model.CanCancel = canCancel;
                model.CanComplete = canComplete;
                model.CanModifyState = canModifyState;
                model.CanModifyQuality = canModifyQuality;
                model.User = user;
                model.Notes = intervention.Notes;
                model.Health = intervention.Health;
                model.Date = intervention.LastVisit ?? DateTime.Today; 

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SiteEngineer, Manager")]
        public ActionResult Edit(EditInterventionViewModel model)
        {

            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                InterventionRepo interventionRepo = new InterventionRepo(db);
                Intervention intervention = interventionRepo.GetAllInterventions().GetInterventions().Where(i => i.ID == model.InterventionID).First();
                var users = new UserRepo(db);
               
                if (Request.Form["Approve"] != null)
                {
                    var user = users.GetUserById<EnetCareUser>(User.Identity.GetUserId());
                    intervention.Approve((IInterventionApprover)user);
                }
                else if(Request.Form["Cancel"] != null)
                {
                    var user = users.GetUserById<EnetCareUser>(User.Identity.GetUserId());
                    intervention.Cancel((IInterventionApprover)user);
                }
                else if(Request.Form["Complete"] != null)
                {
                    var user = users.GetUserById<SiteEngineer>(User.Identity.GetUserId());
                    intervention.Complete(user);
                }
                else if (Request.Form["Save Quality"] != null)
                {
                    var user = users.GetUserById<SiteEngineer>(User.Identity.GetUserId());
                    intervention.UpdateNotes(user, model.Notes);
                    intervention.Quality.LastVisit = model.Date;
                    intervention.Quality.Health = model.Health;
                }
                interventionRepo.Update(intervention);
                return RedirectToAction("Edit");
            }
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