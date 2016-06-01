using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ENETCare.IMS.Users;

using System.Data.Entity.Migrations;

namespace ENETCare.IMS.Data.DataAccess
{
    public class UserRepo : GenericRepo<EnetCareUser>
    {
        public UserRepo(EnetCareDbContext context)
            : base(context, context.Users)
        { }

        private T GetUserByFunc<T>(Func<EnetCareUser, bool> func) where T : EnetCareUser
        {
            EnetCareUser user;

            // If the user is an ILocalizedUser, include "District"
            if (typeof(ILocalizedUser).IsAssignableFrom(typeof(T)))
                user = context.Users.OfType<T>().Include("District").Where(func).SingleOrDefault();
            else
                user = context.Users.Where(func).SingleOrDefault();

            if (user == null)
                throw new InvalidOperationException("A user matching the function could not be found.");

            if (!(user is T))
                throw new InvalidOperationException(String.Format("User {0} is not a {1}", user.Email, typeof(T)));

            return (T)user;
        }

        public T GetUserById<T>(string ID) where T : EnetCareUser
        {
            return GetUserByFunc<T>(u => u.Id == ID);
        }

        public T GetUserByEmail<T>(string email) where T : EnetCareUser
        {
            return GetUserByFunc<T>(u => u.Email == email);
        }

        public void Save(EnetCareUser[] users)
        {
            foreach (EnetCareUser user in users)
            {
                if (user is ILocalizedUser)
                    context.Districts.Attach(((ILocalizedUser)user).District);

                // If a user exists with the same E-Mail, update instead
                context.Users.AddOrUpdate(
                    u => u.Email,
                    user);
            }
            context.SaveChanges();
        }

        public void Save(EnetCareUser user)
        {
            if (user is ILocalizedUser)
            {
                context.Districts.Attach(((ILocalizedUser)user).District);
            }
            context.Users.Add(user);

            context.SaveChanges();
        }


        public void Update(EnetCareUser[] users)
        {
            foreach (EnetCareUser user in users)
            {
                if (user is ILocalizedUser)
                {
                    context.Districts.Attach(((ILocalizedUser)user).District);
                }

                context.Users.AddOrUpdate(user);
            }
            context.SaveChanges();
        }


        public void Update(EnetCareUser user)
        {
            if (user is ILocalizedUser)
            {
                context.Districts.Attach(((ILocalizedUser)user).District);
            }
            context.Users.AddOrUpdate(user);

            context.SaveChanges();
        }

        public List<SiteEngineer> GetAllSiteEngineers()
        {
            return context.Users
                .OfType<SiteEngineer>()
                .Include(e => e.District)
                .OrderBy(d => d.Id)
                .ToList<SiteEngineer>();
        }

        public List<Manager> GetAllManagers()
        {
            return context.Users
                .OfType<Manager>()
                .Include(e => e.District)
                .OrderBy(d => d.Id)
                .ToList<Manager>();
        }
    }
}
