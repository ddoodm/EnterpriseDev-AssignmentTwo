using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ENETCare.IMS;
using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;

using ENETCare.IMS.Data.DataAccess;

namespace ENETCare.IMS.Tests
{
    [TestClass]
    public class UsersTest
    {
        EnetCareDbContext context;
        DistrictRepo districtRepo;

        District testDistrictA;

        [TestInitialize]
        public void Setup()
        {
            context = new EnetCareDbContext();
            districtRepo = new DistrictRepo(context);
            testDistrictA = districtRepo.GetNthDistrict(0);
        }

        [TestCleanup]
        public void Cleanup()
        {
            context.Dispose();
        }

        [TestMethod]
        public void Check_Manager_District()
        {
            Manager testManager = new Manager(
                "Bob Bobson", "bob@mail.com", "TestPass1!", 
                testDistrictA, 50, 50);

            Assert.IsTrue(testManager.District == testDistrictA);
        }

        /// <summary>
        /// Tests user comparison works
        /// </summary>
        [TestMethod]
        public void User_Equality_Test_Success()
        {
            SiteEngineer userA = new SiteEngineer(
                "Markus Roberts", "markus@enet.com", "TestPass1!",
                testDistrictA, 50, 5000);
            SiteEngineer userB = userA;

            // Fail if userA != userB
            Assert.AreEqual(userA, userB);
        }

        /// <summary>
        /// Tests that similar users are not equal
        /// </summary>
        [TestMethod]
        public void User_Equality_Test_Failure()
        {
            // User A and User B are not the same user, though their fields are equal
            SiteEngineer userA = new SiteEngineer("Markus Roberts", "markus@enet.com", "TestPass1!", testDistrictA, 50, 5000);
            SiteEngineer userB = new SiteEngineer("Markas Roberts", "markus@enet.com", "TestPass1!", testDistrictA, 50, 5000);

            // Fail if userA == userB
            Assert.AreNotEqual(userA, userB);
        }

        /// <summary>
        /// Tests creating a site engineer, adding to a list of users then retrieving
        /// the site engineer and comparing its name to the expected value
        /// </summary>
        [TestMethod]
        public void Users_Get_Site_Engineers()
        {
            // User A and User B are not the same user, though their fields are equal
            SiteEngineer userA = new SiteEngineer("Markus Roberts", testDistrictA, 50, 5000);

            Users.Users users = new Users.Users();
            users.Add(userA);

            List<SiteEngineer> siteEngineers = users.GetSiteEngineers();

            Assert.IsTrue(siteEngineers[0].Name == "Markus Roberts");
        }
    }
}
