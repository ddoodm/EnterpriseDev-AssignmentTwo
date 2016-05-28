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
            : base(context, context.UserProfiles)
        { }

        public EnetCareUser GetUserById(int ID)
        {
            return context.UserProfiles
                .Where(d => d.ID == ID)
                .First<EnetCareUser>();
        }

        public SiteEngineer GetNthSiteEngineer(int n)
        {
            return context.UserProfiles
                .OfType<SiteEngineer>()
                .Include(e => e.District)
                .OrderBy(d => d.ID)
                .Skip(n).FirstOrDefault();
        }

        public void Save(EnetCareUser[] users)
        {
            foreach (EnetCareUser user in users)
            {
                if (user is ILocalizedUser)
                    context.Districts.Attach(((ILocalizedUser)user).District);

                context.UserProfiles.Add(user);
            }
            context.SaveChanges();
        }
    }
}
