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
        public UserRepo(EnetCareDbContext context)
            : base(context, context.Users)
        { }

        public EnetCareUser GetUserById(int ID)
        {
            return context.Users
                .Where(d => d.ID == ID)
                .First<EnetCareUser>();
        }

        public SiteEngineer GetNthSiteEngineer(int n)
        {
            return context.Users
                .OfType<SiteEngineer>()
                .Include(e => e.District)
                .OrderBy(d => d.ID)
                .Skip(n).FirstOrDefault();
        }

        public void EraseAllData()
        {
            if (context.Users.Count() < 1) return;
            context.Users.RemoveRange(context.Users);
            context.SaveChanges();
        }

        public void Save(EnetCareUser[] users)
        {
            foreach (EnetCareUser user in users)
            {
                if (user is ILocalizedUser)
                    context.Districts.Attach(((ILocalizedUser)user).District);

                context.Users.Add(user);
            }
            context.SaveChanges();
        }
    }
}
