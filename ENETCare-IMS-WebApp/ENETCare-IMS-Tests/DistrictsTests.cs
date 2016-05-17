using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ENETCare.IMS.Data.DataAccess;

namespace ENETCare.IMS.Tests
{
    [TestClass]
    public class DistrictsTests
    {
        private DistrictRepo districtRepo;

        [TestInitialize]
        public void Setup()
        {
            districtRepo = DistrictRepo.New;
        }

        [TestMethod]
        public void Get_District_By_ID_Returns_Valid_District()
        {
            /*
            int id = 1;
            District district = districtRepo.GetDistrictByID(id);

            Assert.IsTrue(district.ID == id);
            */

            // With EF, we cannot guarantee any primary key, so this test no longer makes sense
            Assert.Fail();
        }
    }
}
