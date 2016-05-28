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

        protected EnetCareUser(string name)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
