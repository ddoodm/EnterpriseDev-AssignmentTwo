using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS.Data.DataAccess
{
    public class DistrictRepo : GenericRepo<District>
    {
        public DistrictRepo(EnetCareDbContext context)
            : base(context, context.Districts)
        { }

        public District GetDistrictById(int ID)
        {
            return context.Districts.Where(d => d.DistrictID == ID).First<District>();
        }

        public District GetNthDistrict(int n)
        {
            return context.Districts.OrderBy(d => d.DistrictID).Skip(n).First<District>();
        }

        public void EraseAllData()
        {
            if (context.Districts.Count() < 1) return;
            context.Districts.RemoveRange(context.Districts);
            context.SaveChanges();
        }

        public void Save(District[] districts)
        {
            foreach(District district in districts)
                context.Districts.Add(district);
            context.SaveChanges();
        }
    }
}
