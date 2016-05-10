using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS.Users
{
    public class SiteEngineer : EnetCareUser, ILocalizedUser, IInterventionApprover, IAdvancedUser
    {
        private const string
            TITLE = "Site Engineer",
            HOMEPAGE = "Interventions";

        public District District { get; private set; }
        public decimal MaxApprovableLabour { get; private set; }
        public decimal MaxApprovableCost { get; private set; }

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

        public SiteEngineer(
            int ID,
            string name,
            District district,
            decimal maxApprovableLabour,
            decimal maxApprovableCost)
            : base(ID, name)
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
