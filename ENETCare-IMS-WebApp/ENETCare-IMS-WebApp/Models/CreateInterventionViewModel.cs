using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ENETCare.IMS;
using ENETCare.IMS.Interventions;

using System.ComponentModel.DataAnnotations;

namespace ENETCare.IMS.WebApp.Models
{
    public class CreateInterventionViewModel
    {
        public InterventionTypes Types { get; set; }
        public Clients Clients { get; set; }

        [Required]
        [Display(Name = "Intervention Type")]
        public int SelectedTypeID { get; set; }

        [Required]
        [Display(Name = "Client")]
        public int SelectedClientID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [DataType(DataType.Currency)]
        public decimal? Cost { get; set; }
        
        [DataType(DataType.Time)]
        public decimal? Labour { get; set; }

        [MaxLength(2500)]
        public string Notes { get; set; }
    }
}