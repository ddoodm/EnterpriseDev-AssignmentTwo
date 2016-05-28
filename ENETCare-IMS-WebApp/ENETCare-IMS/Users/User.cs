using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using System.Configuration;

namespace ENETCare.IMS.Users
{
    public abstract class EnetCareUser : IdentityUser, IEnetCareUser
    {
        [Required]
        public string Name { get; private set; }

        /// <summary>
        /// The User's position (title), ie "Site Engineer"
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// The page to which the User is directed upon log-in
        /// </summary>
        public abstract string HomePage { get; }

        protected EnetCareUser() { }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<EnetCareUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        protected EnetCareUser(string name, string email, string password)
        {
            this.Name = name;
            this.UserName = MakeUsernameFrom(name);
            this.Email = email;
            this.PasswordHash = HashPassword(password);
        }

        /// <summary>
        /// Generates a username from a full name, of the form:
        /// first.last
        /// 
        /// For example:
        /// deinyon.davies
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string MakeUsernameFrom(string name)
        {
            return String.Join(".", name.ToLower().Split(' '));
        }

        private string HashPassword(string password)
        {
            var hasher = new PasswordHasher();
            return hasher.HashPassword(password);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
