using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS.Data.DataAccess
{
    public class DistrictRepo : GenericRepo<District>
    {
        public static DistrictRepo New
        {
            get { return new DistrictRepo(); }
        }

        /// <summary>
        /// NOTICE: This returns a **copy** of the Districts
        /// </summary>
        public Districts AllDistricts
        {
            get
            {
                using (var db = new EnetCareDbContext())
                {
                    return new Districts(db.Districts.ToList<District>());
                }
            }
        }

        public District GetDistrictById(int ID)
        {
            using (var db = new EnetCareDbContext())
            {
                return db.Districts.Where(d => d.DistrictID == ID).First<District>();
            }
        }

        public District GetNthDistrict(int n)
        {
            using (var db = new EnetCareDbContext())
            {
                return db.Districts.OrderBy(d => d.DistrictID).Skip(n).First<District>();
            }
        }

        public void EraseAllData()
        {
            using (var db = new EnetCareDbContext())
            {
                if (db.Districts.Count() < 1) return;
                db.Districts.RemoveRange(db.Districts);
                db.SaveChanges();
            }
        }

        public void Save(District[] districts)
        {
            using (var db = new EnetCareDbContext())
            {
                foreach(District district in districts)
                    db.Districts.Add(district);
                db.SaveChanges();
            }
        }
    }
}
