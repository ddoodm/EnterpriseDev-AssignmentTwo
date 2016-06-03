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

        private T GetSessionUser<T>(EnetCareDbContext context) where T : EnetCareUser
        {
            var repo = new UserRepo(context);
            return repo.GetUserById<T>(User.Identity.GetUserId());
        }

        private IInterventionApprover GetSessionApproverUser(EnetCareDbContext context)
        {
            if (User.IsInRole("SiteEngineer"))
                return GetSessionUser<SiteEngineer>(context);
            if (User.IsInRole("Manager"))
                return GetSessionUser<Manager>(context);

            return null;
        }

        private SiteEngineer GetSessionSiteEngineer(EnetCareDbContext context)
        {
            return GetSessionUser<SiteEngineer>(context);
        }

        // GET: Interventions
        [Authorize(Roles = "SiteEngineer, Manager")]
        public ActionResult Index(InterventionApprovalState? state)
        {
            string interventionsTitle = "Interventions";
            ViewData["Title"] = interventionsTitle;

            // Retrieve Interventions
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                IInterventionApprover user = GetSessionApproverUser(db);

                InterventionRepo repo = new InterventionRepo(db);
                Interventions interventions =
                    repo.GetInterventionHistory(user);

                // Filter by state if a state is supplied
                if (state.HasValue)
                    interventions = interventions.FilterByState(state.Value);

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

                SiteEngineer engineer = GetSessionSiteEngineer(db);

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

                InterventionType type = interventions.GetInterventionTypeById(model.SelectedTypeID);
                Client client = clients.GetClientById(model.SelectedClientID);

                // Obtain the current session's user from the database
                SiteEngineer siteEngineer = GetSessionSiteEngineer(db);

                Intervention intervention = Intervention.Factory.CreateIntervention(
                    type, client, siteEngineer, model.Labour, model.Cost, model.Date);

                interventions.Save(intervention);
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Manager")]
        public ActionResult ViewProposed()
        {
            return RedirectToAction("Index", new { state = InterventionApprovalState.Proposed });
        }

        [Authorize(Roles = "Manager")]
        public ActionResult ViewApproved()
        {
            return RedirectToAction("Index", new { state = InterventionApprovalState.Approved });
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

                UserRepo userRepo = new UserRepo(db);
                var user = 
                    userRepo.GetUserById<EnetCareUser>(User.Identity.GetUserId()) as IInterventionApprover;

                if (Request.Form["Approve"] != null)
                    intervention.Approve((IInterventionApprover)user);

                else if(Request.Form["Cancel"] != null)
                    intervention.Cancel((IInterventionApprover)user);

                else if(Request.Form["Complete"] != null)
                    intervention.Complete(user as SiteEngineer);

                else if (Request.Form["Save Quality"] != null)
                {
                    intervention.UpdateNotes(user as SiteEngineer, model.Notes);
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