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
        private const string
            TITLE = "Site Engineer",
            HOMEPAGE = "Interventions";

        [Required, Column("MaxApprovableLabour")]
        public decimal MaxApprovableLabour { get; private set; }
        [Required, Column("MaxApprovableCost")]
        public decimal MaxApprovableCost { get; private set; }

        public District District { get; protected set; }

        public override string Title
        {
            get
            {
                return TITLE;
            }
        }

        public override string HomePage
        {
            get
            {
                return HOMEPAGE;
            }
        }

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
