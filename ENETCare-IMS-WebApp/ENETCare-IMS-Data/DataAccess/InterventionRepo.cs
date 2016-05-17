﻿using System;
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

        public int InterventionTypeCount
        {
            get
            {
                using (var db = new EnetCareDbContext())
                {
                    return db.InterventionTypes.Count();
                }
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

        public void EraseAllInterventions()
        {
            using (var db = new EnetCareDbContext())
            {
                if (db.Interventions.Count() < 1) return;
                db.Interventions.RemoveRange(db.Interventions);
                db.SaveChanges();
            }
        }

        public void EraseAllInterventionTypes()
        {
            using (var db = new EnetCareDbContext())
            {
                if (db.InterventionTypes.Count() < 1) return;
                db.InterventionTypes.RemoveRange(db.InterventionTypes);
                db.SaveChanges();
            }
        }

        public void Save(Intervention[] interventions)
        {
            using (var db = new EnetCareDbContext())
            {
                foreach (Intervention intervention in interventions)
                    db.Interventions.Add(intervention);
                db.SaveChanges();
            }
        }

        public void Save(InterventionType[] types)
        {
            using (var db = new EnetCareDbContext())
            {
                foreach (InterventionType type in types)
                    db.InterventionTypes.Add(type);
                db.SaveChanges();
            }
        }
    }
}