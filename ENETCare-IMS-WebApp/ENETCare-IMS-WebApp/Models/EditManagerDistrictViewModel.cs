using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ENETCare.IMS.Users;
using System.ComponentModel.DataAnnotations;

namespace ENETCare.IMS.WebApp.Models
{
    public class EditManagerDistrictViewModel
    {
        public List<District> Districts { get; set; }
        public Manager Manager { get; set; }

        public String ManagerID { get; set; }

        [Required]
        [Display(Name = "District")]
        public int SelectedDistrictID { get; set; }
    }
}