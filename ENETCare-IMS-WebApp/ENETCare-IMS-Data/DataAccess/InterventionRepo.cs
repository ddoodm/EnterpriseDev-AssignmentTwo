using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;

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

        private IQueryable<Intervention> FullyLoadedInterventionsDbSet
        {
            get
            {
                return context.Interventions
                  .Include(i => i.InterventionType)
                  .Include(i => i.Client)
                  .Include(i => i.Client.District)
                  .Include(i => i.SiteEngineer)
                  .Include(i => i.SiteEngineer.District)
                  .Include(i => i.Approval)
                  .Include(i => i.Quality);
            }
        }

        public Interventions.Interventions GetInterventionHistory(Client client)
        {
            var query = from intervention in FullyLoadedInterventionsDbSet//context.Interventions
                        where intervention.Client.ID == client.ID
                        orderby intervention.Date
                        select intervention;
            return new Interventions.Interventions(query.ToList<Intervention>());
        }

        public Interventions.Interventions GetInterventionHistory(IInterventionApprover user)
        {
            var query = from intervention in context.Interventions
                        orderby intervention.Date
                        where intervention.SiteEngineer == user ||
                        intervention.ApprovingUser == user
                        select intervention;
            return new Interventions.Interventions(query.ToList<Intervention>());
        }

        public Interventions.Interventions GetAllInterventions()
        {
            return new Interventions.Interventions(
                FullyLoadedInterventionsDbSet.ToList<Intervention>());
        }

        public InterventionTypes GetAllInterventionTypes()
        {
            return new InterventionTypes(
                context.InterventionTypes.ToList<InterventionType>());
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

        public void Save(Intervention[] interventions)
        {
            foreach (Intervention intervention in interventions)
            {
                context.Clients.Attach(intervention.Client);
                context.Users.Attach(intervention.SiteEngineer);
                context.InterventionTypes.Attach(intervention.InterventionType);

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
    }
}
