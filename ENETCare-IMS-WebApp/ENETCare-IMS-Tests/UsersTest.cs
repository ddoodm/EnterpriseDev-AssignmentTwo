using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ENETCare.IMS;
using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;

using ENETCare.IMS.Data.DataAccess;

namespace ENETCare.IMS.Tests
{
    [TestClass]
    public class UsersTest
    {
        DistrictRepo districtRepo;

        District testDistrictA;

        [TestInitialize]
        public void Setup()
        {
            districtRepo = DistrictRepo.New;
            testDistrictA = districtRepo.AllDistricts[0];
        }

        [TestMethod]
        public void Check_Manager_District()
        {
            Manager testManager = new Manager(
                "Bob Bobson", testDistrictA, 50, 50);

            Assert.IsTrue(testManager.District == testDistrictA);
        }

        /// <summary>
        /// Tests user comparison works
        /// </summary>
        [TestMethod]
        public void User_Equality_Test_Success()
        {
            SiteEngineer userA = new SiteEngineer("Markus Roberts", testDistrictA, 50, 5000);
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
            SiteEngineer userA = new SiteEngineer("Markus Roberts", testDistrictA, 50, 5000);
            SiteEngineer userB = new SiteEngineer("Markas Roberts", testDistrictA, 50, 5000);

            // Fail if userA == userB
            Assert.AreNotEqual(userA, userB);
        }
    }
}
