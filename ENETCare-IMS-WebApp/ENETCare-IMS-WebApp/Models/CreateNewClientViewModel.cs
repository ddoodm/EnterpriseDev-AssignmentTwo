using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ENETCare.IMS.WebApp.Models
{
    public class CreateNewClientViewModel
    {
        [Required]
        [MaxLength(64)]
        public string NewClientName { get; set; }

        [Required]
        [MaxLength(512)]
        public string NewLocationName { get; set; }

        public District NewDistrict { get; set; }

        [Required]
        public int NewDistrictID { get; set; }
    }
}