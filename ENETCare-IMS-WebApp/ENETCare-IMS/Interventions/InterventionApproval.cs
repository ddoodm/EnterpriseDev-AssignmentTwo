using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ENETCare.IMS.Users;

namespace ENETCare.IMS.Interventions
{
    public class InterventionApproval
    {
        /// <summary>
        /// Internally maintained state of the Approval
        /// </summary>
        private InterventionApprovalStateWrapper state;

        /// <summary>
        /// Describes the current state of the application
        /// </summary>
        public InterventionApprovalState State
        {
            get { return state.CurrentState; }
        }

        /// <summary>
        /// The user who approved the Intervention,
        /// null if the Intervention has not been approved.
        /// </summary>
        public IInterventionApprover ApprovingUser { get; private set; }

        private Intervention intervention;

        public InterventionApproval(
            InterventionApprovalState state,
            IInterventionApprover approvingUser)
        {
            // Intervention must be linked later
            this.state = new InterventionApprovalStateWrapper(state);
            this.ApprovingUser = approvingUser;
        }

        public InterventionApproval(Intervention intervention)
        {
            if (intervention == null)
                throw new ArgumentNullException("An Intervention Approval must be associated with an instantiated Intervention");

            this.intervention = intervention;
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
            bool outcome = true;
            // A manager cannot modify an approved intervention
            if (State == InterventionApprovalState.Approved)
                if (user is Manager)
                    outcome = false;

            // A manager must work in the same district as the intervention
            if (user is Manager)
                if (user.District != intervention.District)
                    outcome = false;

            // A site engineer must be the site engineer who proposed the intervention
            if (user is SiteEngineer)
                if (user.ID != intervention.SiteEngineer.ID)
                        outcome = false;

            // Must be able to approve *at least* the default labour AND the actual labour
            if (user.MaxApprovableLabour < intervention.MaximumLabour)
                outcome = false;

            // Must be able to approve *at least* the default cost AND the actual cost
            if (user.MaxApprovableCost < intervention.MaximumCost)
                outcome = false;

            return outcome;
        }

        internal void LinkIntervention(Intervention intervention)
        {
            if (this.intervention != null)
                throw new InvalidOperationException("Approval is already linked");
            this.intervention = intervention;
        }
    }
}
