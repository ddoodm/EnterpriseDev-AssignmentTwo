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
        /// <summary>
        /// TODO: Replace this Controller with Navbar creation code that only
        ///       allows a Manager to access the Proposed and Approved 
        ///       Intervention screens.
        /// </summary>
        string accountType = "Manager";

        // GET: Manager
        public ActionResult Index()
        {
            ViewData["Title"] = accountType;

            return View();
        }

    }
}