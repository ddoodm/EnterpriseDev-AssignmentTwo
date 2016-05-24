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

        public ActionResult ViewClient(int ID)
        {
            int id = ID - 1;
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                ClientRepo clientRepo = new ClientRepo(db);
                InterventionRepo interventionRepo = new InterventionRepo(db);

                
                Client client = clientRepo.GetNthClient(id);
                Interventions interventions = interventionRepo.GetInterventionHistory(client);

                return View(new ViewClientInterventionsViewModel()
                {
                    ClientName = client.DescriptiveName,
                    DistrictName = client.District.Name,
                    LocationName = client.Location,
                    Interventions = interventions,

                });

            }
        }

      
                    

    }
}