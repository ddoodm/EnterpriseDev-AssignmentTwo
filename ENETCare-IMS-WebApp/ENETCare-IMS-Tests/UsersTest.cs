using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ENETCare.IMS;
using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;

namespace ENETCare.IMS.Tests
{
    [TestClass]
    public class UsersTest
    {
        ENETCareDAO application;

        [TestInitialize]
        public void Setup()
        {
            application = new ENETCareDAO();
        }

        [TestMethod]
        public void Check_Manager_District()
        {
            Manager testManager = new Manager(1, "Bob Bobson",
                application.Districts.GetDistrictByID(2), 50, 50);

            Assert.IsTrue(testManager.District.Name == "Rural Indonesia");
        }
    }
}
