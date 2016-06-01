using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ENETCare.IMS.Users;
using ENETCare.IMS.Interventions;
using System.ComponentModel.DataAnnotations;

namespace ENETCare.IMS.WebApp.Models
{
    public class EditInterventionViewModel
    {

        public Intervention Intervention { get; set; }
        public EnetCareUser User { get; set; }
        public bool CanModifyState { get; set; }
       
        public bool CanModifyQuality { get; set; }
        public bool CanApprove { get; set; }
        public bool CanCancel { get; set; }
        public bool CanComplete { get; set; } 

        public int InterventionID { get; set; }
        public string UserID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [MaxLength(2500)]
        public string Notes { get; set; }
        
        public decimal Health { get; set; }


    }
}