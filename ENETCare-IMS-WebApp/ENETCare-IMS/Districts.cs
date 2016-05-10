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
        private ENETCareDAO application;

        private List<District> districts = new List<District>();

        public Districts(ENETCareDAO application)
        {
            this.application = application;
        }

        public int Count
        {
            get { return districts.Count; }
        }

        public District this[int index]
        {
            get
        {
               return districts.First<District>(district => district.ID == index); 
            }
        }

        public District GetDistrictByID(int id)
        {
            if (id == 0)
                throw new IndexOutOfRangeException("ENETCare data is 1-indexed, but an index of 0 was requested.");
            return districts.First<District>(d => d.ID == id);
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
