using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ENETCare.IMS;
using ENETCare.IMS.Interventions;

namespace ENETCare.IMS.WebApp.Models
{
    public class ViewClientInterventionsViewModel
    {

        public string ClientName { get; set; }
        public string DistrictName { get; set; }
        public string LocationName { get; set; }
        public ENETCare.IMS.Interventions.Interventions Interventions { get; set; }
    }
}