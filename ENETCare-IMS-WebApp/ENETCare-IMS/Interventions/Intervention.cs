using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ENETCare.IMS.Users;

namespace ENETCare.IMS.Interventions
{
    public class Intervention
    {
        public int ID { get; private set; }

        #region Core Information
        /// <summary>
        /// The type of Intervention to be performed
        /// </summary>
        public InterventionType InterventionType { get; private set; }

        /// <summary>
        /// The client for whom the intervention was created
        /// </summary>
        public Client Client { get; private set; }

        /// <summary>
        /// The Site Engineer who proposed the Intervention
        /// </summary>
        public SiteEngineer SiteEngineer { get; private set; }

        /// <summary>
        /// The date on which the intervention shall be performed
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// The labour required (in hours).
        /// The value is stored as decimal in order to permit fractional values.
        /// </summary>
        public decimal Labour { get; private set; }

        /// <summary>
        /// The projected cost of the Intervention.
        /// Default: interventionType.Cost; can be overridden by the Site Engineer
        /// </summary>
        public decimal Cost { get; private set; }

        /// <summary>
        /// The maximum of this Intervention's actual projected labour,
        /// and the default labour estimate defined in the Intervention's type.
        /// </summary>
        public decimal MaximumLabour
        {
            get { return Math.Max(Labour, InterventionType.Labour); }
        }

        /// <summary>
        /// The maximum of this Intervention's actual projected cost,
        /// and the default cost estimate defined in the Intervention's type.
        /// </summary>
        public decimal MaximumCost
        {
            get { return Math.Max(Cost, InterventionType.Cost); }
        }
        #endregion

        #region Administrative Information

        /// <summary>
        /// Stores an maintains the Approval State of this Intervention
        /// </summary>
        private InterventionApproval approval;

        /// <summary>
        /// Describes the Approval state of this intervention
        /// </summary>
        public InterventionApprovalState ApprovalState
        {
            get { return approval.State; }
        }

        public IInterventionApprover ApprovingUser
        {
            get { return approval.ApprovingUser; }
        }

        #endregion

        #region Quality Management Information

        /// <summary>
        /// Describes the 'progress' and 'health' of the intervention
        /// </summary>
        private InterventionQualityManagement Quality;

        public Percentage Health
        {
            get { return (Quality == null)? null : Quality.Health; }
        }

        public DateTime? LastVisit
        {
            get { return (Quality == null)? (DateTime?)null : Quality.LastVisit; }
        }

        /// <summary>
        /// Notes
        /// </summary>
        public string Notes { get; private set; }

        #endregion

        public District District
        {
            get { return Client.District; }
        }

        /// <summary>
        /// Updates the notes for this Intervention.
        /// Only a Site Engineer is permitted to modify notes.
        /// </summary>
        /// <param name="editorEngineer">The Site Engineer who created this Intervention</param>
        /// <param name="newNotes">The updated notes</param>
        public void UpdateNotes(SiteEngineer editorEngineer, string newNotes)
        {
            if (editorEngineer != this.SiteEngineer)
                throw new InvalidOperationException("Notes may only be edited by the Site Engineer who proposed the Intervention.");

            this.Notes = newNotes;
        }

        public bool UserCanChangeState(IInterventionApprover user)
        {
            return approval.CanChangeState(user);
        }

        public void Approve(IInterventionApprover user)
        {
            approval.Approve(user);
        }

        public void Cancel(IInterventionApprover user)
        {
            approval.Cancel(user);
        }

        public void Complete(SiteEngineer user)
        {
            approval.Complete(user);
        }

        public bool UserCanChangeQuality(EnetCareUser user)
        {
            if (user is SiteEngineer)
            {
                SiteEngineer engineer = (SiteEngineer)user;
                return (engineer.District == SiteEngineer.District);
            }
            else return false;
        }

        private Intervention (
                int ID,
                InterventionType interventionType,
                Client client,
                SiteEngineer siteEngineer,
                decimal labour,
                decimal cost,
                DateTime date)
        {
            this.ID = ID;
            this.InterventionType = interventionType;
            this.Client = client;
            this.SiteEngineer = siteEngineer;
            this.Labour = labour;
            this.Cost = cost;
            this.Date = date;

            // Initialize the Approval
            approval = new InterventionApproval(this);
        }

        public class Factory
        {
            /// <summary>
            /// Instantiates an Intervention given all optional data, and extra data
            /// </summary>
            /// <param name="type">The type of the intervention</param>
            /// <param name="client">The client associated with the intervention</param>
            /// <param name="siteEngineer">The staff proposing the intervention</param>
            /// <param name="labour">The required labour (in hours) - overrides 'type'</param>
            /// <param name="cost">The required cost (in AUD) - overrides 'type'</param>
            /// <param name="date">The date of the intervention - overrides the present date</param>
            /// <param name="notes">Quality control / optional notes</param>
            /// <param name="approval">The object that defines the state of approval of this Intervention</param>
            /// <param name="quality">The object that holds quality control information for this Intervention</param>
            /// <returns>A new Intervention</returns>
            public static Intervention RawCreateIntervention(
                int ID,
                InterventionType type,
                Client client,
                SiteEngineer siteEngineer,
                decimal? labour,
                decimal? cost,
                DateTime date,
                string notes,
                InterventionApproval approval,
                InterventionQualityManagement quality
                )
            {
                Intervention intervention = RawCreateIntervention(
                    ID, type, client, siteEngineer, labour, cost, date);

                // Set extra data
                if (approval != null) intervention.approval = approval;
                intervention.Notes = notes;
                intervention.Quality = quality;

                return intervention;
            }

            public static Intervention RawCreateIntervention (
                int ID,
                InterventionType type,
                Client client,
                SiteEngineer siteEngineer,
                decimal? labour,
                decimal? cost,
                DateTime date
                )
            {
                // Populate labour and cost with type defaults if they are not defined
                if (labour == null) labour = type.Labour;
                if (cost == null) cost = type.Cost;

                return new Intervention(ID, type, client, siteEngineer, labour.Value, cost.Value, date);
            }

            /// <summary>
            /// Instantiates an Intervention given all optional data
            /// </summary>
            /// <param name="type">The type of the intervention</param>
            /// <param name="client">The client associated with the intervention</param>
            /// <param name="siteEngineer">The staff proposing the intervention</param>
            /// <param name="labour">The required labour (in hours) - overrides 'type'</param>
            /// <param name="cost">The required cost (in AUD) - overrides 'type'</param>
            /// <param name="date">The date of the intervention - overrides the present date</param>
            /// <returns>A new Intervention</returns>
            public static Intervention CreateIntervention (
                int ID,
                InterventionType type,
                Client client,
                SiteEngineer siteEngineer,
                decimal? labour,
                decimal? cost,
                DateTime date
                )
            {
                // The Client must exist in the same district as the Engineer.
                // The User Interface should disallow this operation.
                if (client.District != siteEngineer.District)
                    throw new ArgumentException("Cannot create Intervention.\nThe Client must exist in the same district as the Site Engineer.");

                var intervention = RawCreateIntervention(
                    ID, type, client, siteEngineer, labour, cost, date);

                // If possible, auto-approve the intervention on creation
                if (intervention.UserCanChangeState(siteEngineer))
                    intervention.Approve(siteEngineer);

                return intervention;
            }

            /// <summary>
            /// Instantiates an Intervention given no additional data
            /// </summary>
            /// <param name="type">The type of Intervention to create</param>
            /// <param name="client">The client associated with the Intervention</param>
            /// <param name="siteEngineer">The engineer proposing the Intervention</param>
            /// <returns></returns>
            public static Intervention CreateIntervention (
                int ID,
                InterventionType type,
                Client client,
                SiteEngineer siteEngineer
                )
            {
                // Use default values from the specified Intervention Type
                decimal labour = type.Labour;
                decimal cost = type.Cost;

                // Use the present date
                DateTime date = DateTime.Now;

                return CreateIntervention(ID, type, client, siteEngineer, labour, cost, date);
            }
        }
    }
}
