using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ENETCare.IMS.WebApp.Models
{
    public class CreateNewClientViewModel
    {
        public string NewClientName { get; set; }
        public string NewLocationName { get; set; }
        public District NewDistrict { get; set; }
        public int NewDistrictID { get; set; }
    }
}