using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS.Interventions
{
    public enum InterventionApprovalState
    {
        Proposed, Approved, Cancelled, Completed
    }

    public class InterventionApprovalStateWrapper
    {
        public InterventionApprovalStateWrapper() { }
        public InterventionApprovalStateWrapper(InterventionApprovalState initialState)
        {
            this.CurrentState = initialState;
        }

        public InterventionApprovalState CurrentState { get; private set; }

        public void ChangeState(InterventionApprovalState targetState)
        {
            // Check whether the requested state change is permitted
            if (!TryChangeState(targetState))
                throw new InvalidOperationException(
                    String.Format("Cannot change Intervention Approval State from {0} to {1}",
                    Enum.GetName(typeof(InterventionApprovalState), CurrentState),
                    Enum.GetName(typeof(InterventionApprovalState), targetState)));
        }

        public bool TryChangeState(InterventionApprovalState targetState)
        {
            // Permit no change
            if (CurrentState == targetState)
                return true;

            // Use a State Machine to determine permitted state changes
            switch (CurrentState)
            {
                case InterventionApprovalState.Proposed:
                    // Cannot complete a proposed intervention
                    if (targetState == InterventionApprovalState.Completed)
                        return false;
                    break;

                case InterventionApprovalState.Approved:
                    // Cannot propose an approved intervention
                    if (targetState == InterventionApprovalState.Proposed)
                        return false;
                    break;

                case InterventionApprovalState.Cancelled:
                case InterventionApprovalState.Completed:
                    // Cannot modify a cancelled or completed intervention
                    return false;
            }

            // Allow change at this point
            CurrentState = targetState;
            return true;
        }

        public override string ToString()
        {
            return Enum.GetName(typeof(InterventionApprovalState), CurrentState);
        }
    }
}
