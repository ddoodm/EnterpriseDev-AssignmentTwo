using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS.Users
{
    public interface IEnetCareUser
    {
        string Id { get; }

        string Name { get; }

        /// <summary>
        /// The User's position (title), ie "Site Engineer"
        /// </summary>
        string Title { get; }

        /// <summary>
        /// The page to which the User is directed upon log-in
        /// </summary>
        string HomePageAction { get; }
        string HomePageController { get; }

        string Role { get; }
        string Email { get; }
    }
}
