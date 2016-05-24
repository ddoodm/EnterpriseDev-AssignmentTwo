using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS.Interventions
{
    public class InterventionTypes : IReadOnlyList<InterventionType>
    {
        private List<InterventionType> Types { get; set; }

        public InterventionTypes()
        {
            Types = new List<InterventionType>();
        }

        public InterventionTypes(List<InterventionType> types)
        {
            this.Types = types;
        }

        public int Count
        {
            get { return Types.Count; }
        }

        public InterventionType this[int i]
        {
            get { return Types[i]; }
        }

        public IEnumerator<InterventionType> GetEnumerator()
        {
            return Types.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public InterventionType GetTypeByID(int id)
        {
            return Types.First<InterventionType>(c => c.ID == id);
        }

        public void Add(InterventionType type)
        {
            Types.Add(type);
        }
    }
}
