using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ENETCare.IMS.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENETCare.IMS.Interventions
{
    public class InterventionApproval
    {
        public int ID { get; private set; }

        /// <summary>
        /// Internally maintained state of the Approval
        /// </summary>
        [Required]
        public InterventionApprovalStateWrapper state { get; private set; }

        /// <summary>
        /// Describes the current state of the application
        /// </summary>
        public InterventionApprovalState State
        {
            get { return state.CurrentState; }
        }

        /// <summary>
        /// Concrete IInterventionApprover implementations for Entity Framework
        /// </summary>
        public virtual SiteEngineer ApprovingSiteEngineer { get; private set; }
        public virtual Manager ApprovingManager { get; private set; }

        /// <summary>
        /// The user who approved the Intervention,
        /// null if the Intervention has not been approved.
        /// </summary>
        public IInterventionApprover ApprovingUser
        {
            get
            {
                if (ApprovingSiteEngineer != null) return ApprovingSiteEngineer;
                if (ApprovingManager != null) return ApprovingManager;
                return null;
            }

            private set
            {
                if (ApprovingUser != null)
                    throw new InvalidOperationException("The Intervention has already been approved");

                if (value is SiteEngineer) { ApprovingSiteEngineer = (SiteEngineer)value; return; }
                if (value is Manager) { ApprovingManager = (Manager)value; return; }
            }
        }

        public Intervention Intervention { get; private set; }

        private InterventionApproval() { }

        public InterventionApproval(Intervention intervention)
        {
            if (intervention == null)
                throw new ArgumentNullException("An Intervention Approval must be associated with an instantiated Intervention");

            this.Intervention = intervention;
            this.state = new InterventionApprovalStateWrapper();
        }

        public void ChangeState(InterventionApprovalState targetState, IInterventionApprover user)
        {
            // Check that the user can change the state of the Intervention
            if (!CanChangeState(user)) 
                throw new InvalidOperationException("The user is not permitted to change the state of this Intervention");

            // Request to change states. Will throw an exception if current state is invalid.
            this.state.ChangeState(targetState);
        }

        public void Approve(IInterventionApprover user)
        {
            ChangeState(InterventionApprovalState.Approved, user);
            ApprovingUser = user;
        }

        public void Cancel(IInterventionApprover user)
        {
            ChangeState(InterventionApprovalState.Cancelled, user);
        }

        public void Complete(SiteEngineer user)
        {
            ChangeState(InterventionApprovalState.Completed, user);
        }

        public bool CanChangeState(IInterventionApprover user)
        {
            // A manager cannot modify an approved intervention
            if (State == InterventionApprovalState.Approved)
                if (user is Manager)
                    return false;

            // A manager must work in the same district as the intervention
            if (user is Manager)
                if (user.District != Intervention.District)
                    return false;

            // A site engineer must be the site engineer who proposed the intervention
            if (user is SiteEngineer)
                if (((SiteEngineer)user) != Intervention.SiteEngineer)
                    return false;

            // From the proposed state, an intervention may only be modified by a user who can afford it
            if (State == InterventionApprovalState.Proposed)
            {
                // Must be able to approve *at least* the default labour AND the actual labour
                if (user.MaxApprovableLabour < Intervention.MaximumLabour)
                    return false;

                // Must be able to approve *at least* the default cost AND the actual cost
                if (user.MaxApprovableCost < Intervention.MaximumCost)
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            if (State == InterventionApprovalState.Approved)
                return String.Format("Approved by {0}", ApprovingUser.Name);

            return State.ToString();
        }
    }
}
