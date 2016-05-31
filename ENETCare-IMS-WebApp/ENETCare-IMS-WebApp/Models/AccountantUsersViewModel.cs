using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ENETCare.IMS.Users;

namespace ENETCare.IMS.WebApp.Models
{
    public class AccountantUsersViewModel
    {
        public List<SiteEngineer> Engineers { get; set; }

        public List<Manager> Managers { get; set; }

    }
}