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
        public override string Title { get { return "Manager"; } }
        public override string HomePageAction { get { return "Index"; } }
        public override string HomePageController { get { return "Manager"; } }
        public override string Role { get { return "Manager"; } }

        [Required, Column("MaxApprovableLabour")]
        public decimal MaxApprovableLabour { get; private set; }
        [Required, Column("MaxApprovableCost")]
        public decimal MaxApprovableCost { get; private set; }

        public District District { get; protected set; }

        private Manager() { }

        public Manager(
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
