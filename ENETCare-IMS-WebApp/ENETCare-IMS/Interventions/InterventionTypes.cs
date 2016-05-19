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
        private List<InterventionType> types;

        public InterventionTypes()
        {
            types = new List<InterventionType>();
        }

        public InterventionTypes(List<InterventionType> types)
        {
            this.types = types;
        }

        public int Count
        {
            get { return types.Count; }
        }

        public InterventionType this[int i]
        {
            get { return types[i]; }
        }

        public IEnumerator<InterventionType> GetEnumerator()
        {
            return types.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public InterventionType GetTypeByID(int id)
        {
            return types.First<InterventionType>(c => c.ID == id);
        }

        public void Add(InterventionType type)
        {
            types.Add(type);
        }
    }
}
