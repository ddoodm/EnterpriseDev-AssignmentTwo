using System;
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

        [Required]
        public decimal MaxApprovableLabour { get; private set; }
        [Required]
        public decimal MaxApprovableCost { get; private set; }

        public ICollection<Intervention> Interventions { get; private set; }

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

        public SiteEngineer() { }

        public SiteEngineer(
            string name,
            District district,
            decimal maxApprovableLabour,
            decimal maxApprovableCost)
            : base(name)
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
