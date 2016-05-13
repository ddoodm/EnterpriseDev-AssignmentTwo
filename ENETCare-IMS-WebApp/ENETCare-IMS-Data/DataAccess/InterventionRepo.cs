using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;

using ENETCare.IMS.Interventions;

namespace ENETCare.IMS.Data.DataAccess
{
    public class InterventionRepo : GenericRepo<Intervention>
    {
        public static InterventionRepo New
        {
            get { return new InterventionRepo(); }
        }

        public void Save(Intervention intervention)
        {
            using (var db = new EnetCareDbContext())
            {
                db.Interventions.Add(intervention);
                db.SaveChanges();
            }
        }

        public InterventionTypes AllInterventionTypes
        {
            get
            {
                using (var db = new EnetCareDbContext())
                {
                    return new InterventionTypes(
                        db.InterventionTypes.ToList<InterventionType>());
                }
            }
        }

        public Interventions.Interventions AllInterventions
        {
            get
            {
                using (var db = new EnetCareDbContext())
                {
                    return new Interventions.Interventions(
                        db.Interventions.ToList<Intervention>());
                }
            }
        }

        public Interventions.Interventions GetInterventionHistory(Client client)
        {
            using (var db = new EnetCareDbContext())
            {
                var query = from intervention in db.Interventions
                            orderby intervention.Date
                            select intervention;
                return new Interventions.Interventions(query.ToList<Intervention>());
            }
        }
    }
}
