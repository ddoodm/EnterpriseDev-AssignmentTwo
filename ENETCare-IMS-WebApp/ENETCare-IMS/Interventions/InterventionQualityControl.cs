using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS.Interventions
{
    public class InterventionQualityManagement
    {
        /// <summary>
        /// The health of the work completed for the Intervention.
        /// Stored as a percentage from 0 to 100.
        /// Values outside 0 to 100 are clamped.
        /// </summary>
        public Percentage Health;

        public DateTime LastVisit;

        public InterventionQualityManagement(Percentage Health, DateTime LastVisit)
        {
            this.Health = Health;
            this.LastVisit = LastVisit;
        }

        public InterventionQualityManagement()
        {
            Health = 100.0m;
        }
    }
}
