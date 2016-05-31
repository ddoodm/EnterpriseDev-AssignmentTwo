using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ENETCare.IMS.Users;

namespace ENETCare.IMS.WebApp.Models
{
    public class EditManagerDistrictViewModel
    {
        public List<District> Districts { get; set; }
        public Manager Manager { get; set; }

        public String ManagerID { get; set; }
        public int SelectedDistrictID { get; set; }
    }
}