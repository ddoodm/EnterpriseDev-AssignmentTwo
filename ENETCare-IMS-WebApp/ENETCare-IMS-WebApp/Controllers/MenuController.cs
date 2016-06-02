using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ENETCare_IMS_WebApp.Controllers
{
    public class MenuController : Controller
    {
        public ActionResult MenuBar()
        {
            return PartialView();
        }
    }
}