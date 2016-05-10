using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ENETCare.IMS.Tests
{
    [TestClass]
    public class DistrictsTests
    {
        private ENETCareDAO application;

        [TestInitialize]
        public void Setup()
        {
            application = new ENETCareDAO();
        }

        [TestMethod]
        public void Get_District_By_ID_Returns_Valid_District()
        {
            int id = 1;
            District district = application.Districts.GetDistrictByID(id);

            Assert.IsTrue(district.ID == id);
        }
    }
}
