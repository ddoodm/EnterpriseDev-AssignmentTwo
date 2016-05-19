using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ENETCare.IMS.Users;

namespace ENETCare.IMS.Interventions
{
    public class Interventions : IReadOnlyList<Intervention>
    {
        private List<Intervention> interventions = new List<Intervention>();

        public Interventions()
        {
            interventions = new List<Intervention>();
        }

        public Interventions(List<Intervention> list)
        {
            this.interventions = list;
        }

        public Intervention CreateIntervention(
            InterventionType type,
            Client client,
            SiteEngineer siteEngineer,
            DateTime date,
            decimal? cost,
            decimal? labour,
            string notes)
        {
            Intervention newIntervention = Intervention.Factory.CreateIntervention(
                type, client, siteEngineer, labour, cost, date);
            newIntervention.UpdateNotes(siteEngineer, notes);

            Add(newIntervention);
            return newIntervention;
        }

        public void Add(Intervention intervention)
        {
            interventions.Add(intervention);
        }

        public int Count
        {
            get { return interventions.Count; }
        }

        /// <summary>
        /// Retrieves the Intervention with the given ID
        /// </summary>
        /// <param name="ID">The ID of the Intervention to retrieve</param>
        /// <returns>The Intervention with the given ID</returns>
        public Intervention this[int ID]
        {
            get
            {
                if (ID == 0)
                    throw new IndexOutOfRangeException("ENETCare data is 1-indexed, but an index of 0 was requested.");
                return interventions.First<Intervention>(
                    intervention => intervention.ID == ID);
            }
        }

        public Interventions GetInterventionsWithClient(Client client)
        {
            return new Interventions(interventions
                .Where(x => x.Client.ID == client.ID)
                .ToList<Intervention>());
        }

        public Interventions FilterByDistrict(District district)
        {
            return new Interventions(interventions
                .Where(x => x.District == district)
                .ToList<Intervention>());
        }

        public Interventions FilterByState(InterventionApprovalState state)
        {
            return new Interventions(interventions
                .Where(x => x.ApprovalState == state)
                .ToList<Intervention>());
        }

        public Interventions FilterForUserDisplay(IInterventionApprover user)
        {
            return new Interventions(interventions
                .Where(x =>
                {
                    bool sameDistrict = x.District == user.District;
                    bool sameProposer = x.SiteEngineer.ID == user.ID;

                    bool sameApprover = x.ApprovingUser == null? false :
                        x.ApprovingUser.ID == user.ID;

                    return sameDistrict || sameProposer || sameApprover;
                })
                .ToList<Intervention>());
        }

        public IEnumerator<Intervention> GetEnumerator()
        {
            return interventions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<Intervention> GetInterventions()
        {
            return interventions;
        }
    }
}
