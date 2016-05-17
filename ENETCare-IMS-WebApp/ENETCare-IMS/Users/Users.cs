﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS.Users
{
    public class Users : IReadOnlyList<EnetCareUser>
    {
        private List<EnetCareUser> users;

        public Users()
        {
            users = new List<EnetCareUser>();
        }

        public void Add(EnetCareUser user)
        {
            users.Add(user);
        }

        public void Add(Users newUsers)
        {
            foreach (EnetCareUser user in newUsers)
                users.Add(user);
        }

        public SiteEngineer CreateSiteEngineer(
            string name, District district, decimal maxApprovableLabour, decimal maxApprovableCost)
        {
            SiteEngineer engineer =
                new SiteEngineer(name, district, maxApprovableLabour, maxApprovableCost);
            Add(engineer);
            return engineer;
        }

        public Manager CreateManager(
            string name, District district, decimal maxApprovableLabour, decimal maxApprovableCost)
        {
            Manager manager =
                new Manager(name, district, maxApprovableLabour, maxApprovableCost);
            Add(manager);
            return manager;
        }

        public Accountant CreateAccountant(string name)
        {
            Accountant accountant = new Accountant(name);
            Add(accountant);
            return accountant;
        }

        public EnetCareUser GetUserByID(int ID)
        {
            if (ID == 0)
                throw new IndexOutOfRangeException("ENETCare data is 1-indexed, but an index of 0 was requested.");
            return users.First<EnetCareUser>(
                user => user.ID == ID);
        }

        public IEnumerator<EnetCareUser> GetEnumerator()
        {
            return users.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count { get { return users.Count; } }

        public EnetCareUser this[int index]
        {
            get { return users[index]; }
        }

        public List<EnetCareUser> GetUsers()
        {
            return users;
        }

        public List<SiteEngineer> GetSiteEngineers()
        {
            List<EnetCareUser> siteEngineerUsers = users.Where(user => user is SiteEngineer).ToList();
            List<SiteEngineer> siteEngineers = new List<SiteEngineer>();
            foreach (EnetCareUser user in siteEngineerUsers)
            {
                siteEngineers.Add(user as SiteEngineer);
            }

            return siteEngineers;
        }

        public List<Manager> GetManagers()
        {
            List<EnetCareUser> managerUsers = users.Where(user => user is Manager).ToList();
            List<Manager> managers = new List<Manager>();
            foreach (EnetCareUser user in managerUsers)
            {
                managers.Add(user as Manager);
            }

            return managers;
        }

        public List<IAdvancedUser> GetIAdvancedUsers()
        {
            List<IAdvancedUser> advancedUsers = new List<IAdvancedUser>();
            
            foreach (EnetCareUser user in users)
            {
                if (user is IAdvancedUser)
                {
                    advancedUsers.Add(user as IAdvancedUser);
                }
            }

            return advancedUsers;
        }
        
    }
}
