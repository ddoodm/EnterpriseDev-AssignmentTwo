using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ENETCare.IMS.Users;

namespace ENETCare.IMS.Data.DataAccess
{
    public class UserRepo : GenericRepo<EnetCareUser>
    {
        public static UserRepo New
        {
            get { return new UserRepo(); }
        }

        public EnetCareUser GetUserById(int ID)
        {
            using (var db = new EnetCareDbContext())
            {
                return db.Users.Where(d => d.ID == ID).First<EnetCareUser>();
            }
        }

        public SiteEngineer GetNthSiteEngineer(int n)
        {
            using (var db = new EnetCareDbContext())
            {
                return db.Users
                    .OfType<SiteEngineer>().Include(e => e.District)
                    .OrderBy(d => d.ID)
                    .Skip(n).FirstOrDefault();
            }
        }

        public void EraseAllData()
        {
            using (var db = new EnetCareDbContext())
            {
                if (db.Users.Count() < 1) return;
                db.Users.RemoveRange(db.Users);
                db.SaveChanges();
            }
        }

        public void Save(EnetCareUser[] users)
        {
            using (var db = new EnetCareDbContext())
            {
                foreach (EnetCareUser user in users)
                {
                    if (user is ILocalizedUser)
                        db.Districts.Attach(((ILocalizedUser)user).District);

                    db.Users.Add(user);
                }
                db.SaveChanges();
            }
        }
    }
}
