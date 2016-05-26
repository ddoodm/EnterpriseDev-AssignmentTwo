using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ENETCare.IMS;
using ENETCare.IMS.Data.DataAccess;
using ENETCare.IMS.Interventions;

using ENETCare.IMS.WebApp.Models;

namespace ENETCare_IMS_WebApp.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult EditIntervention(int ID)
        {
            return View();
        }
    }
}