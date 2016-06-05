using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;
using LinqKit;

using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;

namespace ENETCare.IMS.Data.DataAccess
{
    public class InterventionRepo : GenericRepo<Intervention>
    {
        public InterventionRepo(EnetCareDbContext context)
            : base(context, context.Interventions)
        { }

        public int InterventionTypeCount
        {
            get { return context.InterventionTypes.Count();  }
        }

        public Interventions.Interventions GetInterventionHistory(Client client)
        {
            var query = from intervention in context.FullyLoadedInterventions
                        where intervention.Client.ID == client.ID
                        orderby intervention.Date
                        select intervention;
            return new Interventions.Interventions(query.ToList<Intervention>());
        }

        public Interventions.Interventions GetInterventionHistory(IInterventionApprover user)
        {
            // Construct a dynamic LINQ to SQL query using a Predicate Builder
            var predicate = PredicateBuilder.False<Intervention>();

            // Dynamically select approving Site Engineer or Manager,
            // for we may not use the dynamic property "ApprovingUser" in SQL.
            if (user is SiteEngineer)
            {
                // The proposing site engineer is the user
                predicate = predicate.Or(i => i.SiteEngineer.Id == user.Id);

                // Or the approver is the site engineer
                predicate = predicate.Or(i => 
                    i.Approval.ApprovingSiteEngineer != null
                    && i.Approval.ApprovingSiteEngineer.Id == user.Id);
            }
            else if (user is Manager)
                predicate = predicate.Or(i => i.Approval.ApprovingManager != null &&
                i.Approval.ApprovingManager.Id == user.Id);

            // ... Or the Intervention's district is the same as the user's
            predicate = predicate.Or(i => i.Client.DistrictID == user.District.DistrictID);

            var selection = context.FullyLoadedInterventions.AsExpandable().Where(predicate);
            var resultList = selection.ToList<Intervention>();

            // Never recall cancelled interventions (use property)
            resultList = resultList
                .Where(i => i.ApprovalState != InterventionApprovalState.Cancelled)
                .ToList<Intervention>();

            return new Interventions.Interventions(resultList);
        }

        public Interventions.Interventions GetAllInterventions()
        {
            return new Interventions.Interventions(
                context.FullyLoadedInterventions.ToList<Intervention>());
        }

        public Interventions.Interventions GetInterventionsByDistrict(District district)
        {
            return new Interventions.Interventions(
                context.FullyLoadedInterventions.ToList<Intervention>().Where(t => t.District.DistrictID == district.DistrictID).ToList<Intervention>());
        }

        public InterventionTypes GetAllInterventionTypes()
        {
            return new InterventionTypes(
                context.InterventionTypes.ToList<InterventionType>());
        }
        public Intervention GetInterventionByID(int ID)
        {
            return GetAllInterventions().GetInterventions().Where(i => i.ID == ID).First();
        }
        public InterventionType GetInterventionTypeById(int ID)
        {
            return context.InterventionTypes
                .SingleOrDefault<InterventionType>(t => t.ID == ID);
        }

        public InterventionType GetNthInterventionType(int n)
        {
            return context.InterventionTypes
                .OrderBy(i => i.ID).Skip(n)
                .First<InterventionType>();
        }

        public override void EraseAllData()
        {
            EraseAllInterventions();
            EraseAllInterventionTypes();
        }

        public void EraseAllInterventions()
        {
            if (context.Interventions.Count() < 1) return;
            context.Interventions.RemoveRange(context.Interventions);
            context.SaveChanges();
        }

        public void EraseAllInterventionTypes()
        {
            if (context.InterventionTypes.Count() < 1) return;
            context.InterventionTypes.RemoveRange(context.InterventionTypes);
            context.SaveChanges();
        }

        private void AttachNewInterventionToContext(Intervention intervention)
        {
            context.Clients.Attach(intervention.Client);
            context.Users.Attach(intervention.SiteEngineer);
            context.InterventionTypes.Attach(intervention.InterventionType);
        }

        public void Save(Intervention intervention)
        {
            AttachNewInterventionToContext(intervention);
            context.Interventions.Add(intervention);
            context.SaveChanges();
        }

        public void Save(Intervention[] interventions)
        {
            foreach (Intervention intervention in interventions)
            {
                AttachNewInterventionToContext(intervention);
                context.Interventions.Add(intervention);
            }

            context.SaveChanges();
        }
            
        public void Save(InterventionType[] types)
        {
            foreach (InterventionType type in types)
                context.InterventionTypes.Add(type);
            context.SaveChanges();
        }

        public void Update(Intervention intervention)
        {
            AttachNewInterventionToContext(intervention);
            context.Interventions.Attach(intervention);
            context.SaveChanges();
        }

    }
}
