using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ENETCare.IMS.Users;
using System.ComponentModel.DataAnnotations;

namespace ENETCare.IMS.WebApp.Models
{
    public class EditSiteEngineerDistrictViewModel
    {
        public List<District> Districts { get; set; }
        public SiteEngineer Engineer { get; set; }

        public String EngineerID { get; set; }

        [Required]
        [Display(Name = "District")]
        public int SelectedDistrictID { get; set; }
    }
}