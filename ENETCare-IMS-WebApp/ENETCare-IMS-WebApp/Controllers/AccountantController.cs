using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ENETCare_IMS_WebApp.Controllers
{
    public class AccountantController : Controller
    {
        // GET: Accountant
        public ActionResult Index()
        {
            string accountType = "Accountant";
            ViewData["Title"] = accountType;

            return View();
        }
    }
}