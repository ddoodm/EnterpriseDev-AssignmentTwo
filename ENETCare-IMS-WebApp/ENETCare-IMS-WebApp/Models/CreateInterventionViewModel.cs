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
        public InterventionType SelectedType { get; set; }

        [Required]
        public Client SelectedClient { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
    }
}