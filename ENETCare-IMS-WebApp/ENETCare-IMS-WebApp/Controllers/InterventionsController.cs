﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ENETCare.IMS;
using ENETCare.IMS.Data.DataAccess;
using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;
using ENETCare.IMS.MailService;

using ENETCare.IMS.WebApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ENETCare_IMS_WebApp.Controllers
{
    [Authorize(Roles = "SiteEngineer, Manager")]
    public class InterventionsController : Controller
    {
        private EnetCareDbContext DbContext { get; set; }
        private ApplicationUserManager UserManager { get; set; }

        public InterventionsController()
            : base()
        {
            DbContext = new EnetCareDbContext();
            UserManager = new ApplicationUserManager(new UserStore<EnetCareUser>(DbContext));
        }

        public InterventionsController(EnetCareDbContext customContext)
            : base()
        {
            DbContext = customContext;
            UserManager = new ApplicationUserManager(new UserStore<EnetCareUser>(DbContext));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                DbContext.Dispose();

            base.Dispose(disposing);
        }

        // GET: Interventions
        [Authorize(Roles = "SiteEngineer, Manager")]
        public ActionResult Index(InterventionApprovalState? state)
        {
            string interventionsTitle = "Interventions";
            ViewData["Title"] = interventionsTitle;

            // Retrieve Interventions
            IInterventionApprover user = 
                ControllerGetUserUtility.GetSessionApproverUser(DbContext, User);

            InterventionRepo repo = new InterventionRepo(DbContext);
            Interventions interventions =
                repo.GetInterventionHistory(user);

            // Filter by state if a state is supplied
            if (state.HasValue)
                interventions = interventions.FilterByState(state.Value);

            return View(interventions);
        }

        [Authorize(Roles = "SiteEngineer")]
        public ActionResult CreateIntervention()
        {
            var interventionRepo = new InterventionRepo(DbContext);
            var clientRepo = new ClientRepo(DbContext);

            SiteEngineer engineer =
                ControllerGetUserUtility.GetSessionSiteEngineer(DbContext, User);

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

        /// <summary>
        /// Creates a new intervention according to the user-submitted data
        /// and saves it.
        /// </summary>
        /// <param name="model">ViewModel with intervention details</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SiteEngineer")]
        public ActionResult CreateIntervention(CreateInterventionViewModel model)
        {
            // Load type first; required for validation
            InterventionRepo interventions = new InterventionRepo(DbContext);
            InterventionType type =
                model.SelectedTypeID == 0? null :
                interventions.GetInterventionTypeById(model.SelectedTypeID);

            // Display validation errors
            CheckLaborAndCostError(model, type);
            if (!ModelState.IsValid)
                return CreateIntervention();

            // Get the client specified in the model
            ClientRepo clients = new ClientRepo(DbContext);
            Client client = clients.GetClientById(model.SelectedClientID);

            // Obtain the current session's user from the database
            SiteEngineer siteEngineer =
                ControllerGetUserUtility.GetSessionSiteEngineer(DbContext, User);

            // Build the intervention
            Intervention intervention = Intervention.Factory.CreateIntervention(
                type, client, siteEngineer, model.Labour, model.Cost, model.Date);

            //Save notes to intervention
            intervention.UpdateNotes(siteEngineer, model.Notes);

            //Save last visit
            intervention.Quality.LastVisit = model.Date;

            //Save intervention in database
            interventions.Save(intervention);

            return RedirectToAction("Index");
        }

        private void CheckLaborAndCostError(CreateInterventionViewModel model, InterventionType type)
        {
            // Do not validate type if the type has not been selected
            if (type == null)
                return;

            // If user has entered a Labor value less than the type's default...
            if (model.Labour < type.Labour)
                ModelState.AddModelError("LaborLessThanType", "Please enter an amount greater than or equal to " + type.Labour);

            // If user has entered a Cost value less than the type's default...
            if (model.Cost < type.Cost)
                ModelState.AddModelError("CostLessThanType", "Please enter an amount greater than or equal to " + type.Cost);
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
            InterventionRepo interventionRepo = new InterventionRepo(DbContext);
            Intervention intervention = interventionRepo.GetInterventionByID(ID);

            IInterventionApprover user =
                ControllerGetUserUtility.GetSessionApproverUser(DbContext, User);

            bool canModifyQuality = intervention.UserCanChangeQuality(user as EnetCareUser);
            bool canApprove = intervention.UserCanChangeState(user, InterventionApprovalState.Approved);
            bool canCancel = intervention.UserCanChangeState(user, InterventionApprovalState.Cancelled);
            bool canComplete = intervention.UserCanChangeState(user, InterventionApprovalState.Completed);

            EditInterventionViewModel model = new EditInterventionViewModel();
            model.Intervention = intervention;
            model.CanApprove = canApprove;
            model.CanCancel = canCancel;
            model.CanComplete = canComplete;
            model.CanModifyState = true;
            model.CanModifyQuality = canModifyQuality;
            model.User = user;
            model.Notes = intervention.Notes;
            model.Health = intervention.Health;
            model.Date = intervention.LastVisit ?? DateTime.Today;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SiteEngineer, Manager")]
        public ActionResult Edit(EditInterventionViewModel model)
        {
            if (!ModelState.IsValid)
                return Edit(model.InterventionID);

            InterventionRepo interventionRepo = new InterventionRepo(DbContext);
            Intervention intervention = interventionRepo.GetInterventionByID(model.InterventionID);

            UserRepo userRepo = new UserRepo(DbContext);
            var user = userRepo.GetUserById<EnetCareUser>(User.Identity.GetUserId()) as IInterventionApprover;

            // Quality update case
            if (Request.Form["Save Quality"] != null)
            {
                intervention.UpdateNotes(user as SiteEngineer, model.Notes);
                intervention.Quality.LastVisit = model.Date;
                intervention.Quality.Health = model.Health;

                interventionRepo.Save(intervention);
                return RedirectToAction("Edit");
            }

            // State change cases
            if (Request.Form["Approve"] != null)
                intervention.Approve(user);

            else if (Request.Form["Cancel"] != null)
                intervention.Cancel(user);

            else if (Request.Form["Complete"] != null)
                intervention.Complete(user as SiteEngineer);

            // Deliver a notification E-Mail to the proposing engineer (on a new thread)
            Task.Run(() => new EnetCareMailer().NotifyEngineerOfStateChange(intervention));

            interventionRepo.Save(intervention);
            return RedirectToAction("Edit");
        }

        /// <summary>
        /// Returns Intervention Type details as a JSON object.
        /// Used in Create Intervention AJAX transaction.
        /// </summary>
        [HttpPost]
        public JsonResult InterventionType(int ID)
        {
            InterventionRepo repo = new InterventionRepo(DbContext);
            InterventionType type = repo.GetInterventionTypeById(ID);
            return Json(type);
        }
    }
}