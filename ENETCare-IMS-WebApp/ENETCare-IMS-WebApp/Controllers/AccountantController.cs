using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ENETCare_IMS_WebApp.Controllers
{
    public class AccountantController : Controller
    {

        string accountType = "Accountant";


        // GET: Accountant
        public ActionResult Index()
        {
            ViewData["Title"] = accountType;

            return View();
        }

        public ActionResult Report()
        {
            ViewData["Title"] = accountType;

            return View();
        }

        public ActionResult ViewList()
        {
            ViewData["Title"] = accountType;

            return View();
        }

    }
}