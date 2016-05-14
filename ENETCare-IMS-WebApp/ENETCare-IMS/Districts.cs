using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS
{
    public class Districts : IReadOnlyList<District>
    {
        private List<District> districts = new List<District>();

        public Districts(List<District> districts)
        {
            this.districts = districts;
        }

        public int Count
        {
            get { return districts.Count; }
        }

        /// <summary>
        /// Returns the District at the index withing *this* collection.
        /// The 'index' is not the District ID.
        /// </summary>
        /// <param name="index">The index in this collection, not the District ID</param>
        public District this[int index]
        {
            get
            {
                return districts[index];
            }
        }

        public List<District> GetListCopy()
        {
            return new List<District>(districts);
        }

        public IEnumerator<District> GetEnumerator()
        {
            return districts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(District district)
        {
            districts.Add(district);
        }
    }
}
