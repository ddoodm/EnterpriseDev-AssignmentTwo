﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ENETCare.IMS.Interventions;

namespace ENETCare.IMS.Users
{
    public class SiteEngineer : EnetCareUser, ILocalizedUser, IInterventionApprover, IAdvancedUser
    {
        public override string Title { get { return "Site Engineer"; } }
        public override string HomePageAction { get { return "Index"; } }
        public override string HomePageController { get { return "Interventions"; } }

        public override string Role { get { return "SiteEngineer"; } }

        [Required, Column("MaxApprovableLabour")]
        public virtual decimal MaxApprovableLabour { get; private set; }
        [Required, Column("MaxApprovableCost")]
        public virtual decimal MaxApprovableCost { get; private set; }

        [Column("DistrictID"), ForeignKey("District")]
        public int? DistrictID { get; private set; }
        public virtual District District { get; protected set; }

        private SiteEngineer() { }

        public SiteEngineer(
            string name,
            string email,
            string password,
            District district,
            decimal maxApprovableLabour,
            decimal maxApprovableCost)
            : base(name, email, password)
        {
            this.District = district;
            this.MaxApprovableLabour = maxApprovableLabour;
            this.MaxApprovableCost = maxApprovableCost;
        }

        public void UpdateDistrict(District district)
        {
            District = district;
        }
    }
}
