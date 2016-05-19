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
    public class Manager : EnetCareUser, ILocalizedUser, IInterventionApprover, IAdvancedUser
    {
        private const string
            TITLE = "Manager",
            HOMEPAGE = "ProposedInterventions";

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

        public Manager() { }

        public Manager(
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
