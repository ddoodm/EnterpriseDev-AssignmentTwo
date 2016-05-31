using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ENETCare.IMS.WebApp.Models;
using ENETCare.IMS.Data.DataAccess;

namespace ENETCare_IMS_WebApp.Controllers
{
    /// <summary>
    /// TODO: Replace this Controller with Users Controller for
    ///       showing Users, and Navbar creation code that only
    ///       allows an Accountant to access the Report and 
    ///       Users screens.
    /// </summary>
    public class AccountantController : Controller
    {
        string accountType = "Accountant";

        // GET: Accountant
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
    }
}