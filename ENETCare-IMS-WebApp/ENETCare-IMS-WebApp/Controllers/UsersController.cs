using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ENETCare.IMS.WebApp.Models;
using ENETCare.IMS;
using ENETCare.IMS.Users;
using ENETCare.IMS.Data.DataAccess;

namespace ENETCare_IMS_WebApp.Controllers
{
    [Authorize(Roles = "Accountant")]
    public class UsersController : Controller
    {
        string accountType = "Accountant";

        // GET: Users
        public ActionResult Index()
        {
            ViewData["Title"] = accountType;

            AccountantUsersViewModel model = new AccountantUsersViewModel();

            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                UserRepo repo = new UserRepo(db);

                model.Engineers = repo.GetAllSiteEngineers();
                model.Managers = repo.GetAllManagers();

                return View(model);
            }
        }

        public ActionResult EditSiteEngineer(string ID)
        {
            EditSiteEngineerDistrictViewModel model = new EditSiteEngineerDistrictViewModel();
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                DistrictRepo districtRepo = new DistrictRepo(db);
                UserRepo userRepo = new UserRepo(db);

                SiteEngineer engineer = userRepo.GetUserById<SiteEngineer>(ID);
                List<District> districts = districtRepo.GetAllDistricts().Where(district => district.DistrictID != engineer.District.DistrictID).ToList<District>();

                model.Engineer = engineer;
                model.Districts = districts;
                return View(model);
            }
             
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSiteEngineer(EditSiteEngineerDistrictViewModel model)
        {
            // Display validation errors
            if (!ModelState.IsValid)
                return EditSiteEngineer(model.Engineer.Id);

            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                DistrictRepo repo = new DistrictRepo(db);
                UserRepo userRepo = new UserRepo(db);

                District district = repo.GetDistrictById(model.SelectedDistrictID);
                SiteEngineer engineer = userRepo.GetUserById<SiteEngineer>(model.EngineerID);

                engineer.UpdateDistrict(district);

                userRepo.Update(engineer);
            }

            return RedirectToAction("Index", "Accountant");
        }

        public ActionResult EditManager(String ID)
        {
            EditManagerDistrictViewModel model = new EditManagerDistrictViewModel();
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                DistrictRepo districtRepo = new DistrictRepo(db);
                UserRepo userRepo = new UserRepo(db);

                Manager manager = userRepo.GetUserById<Manager>(ID);
                List<District> districts = districtRepo.GetAllDistricts().Where(district => district.DistrictID != manager.District.DistrictID).ToList<District>();

                model.Manager = manager;
                model.Districts = districts;
                return View(model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditManager(EditManagerDistrictViewModel model)
        {
            // Display validation errors
            if (!ModelState.IsValid)
                return EditSiteEngineer(model.Manager.Id);

            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                DistrictRepo repo = new DistrictRepo(db);
                UserRepo userRepo = new UserRepo(db);

                District district = repo.GetDistrictById(model.SelectedDistrictID);
                Manager manager = userRepo.GetUserById<Manager>(model.ManagerID);

                manager.UpdateDistrict(district);

                userRepo.Update(manager);
            }

            return RedirectToAction("Index", "Accountant");
        }
    }
}