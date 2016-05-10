using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ENETCare.IMS.Tests
{
    [TestClass]
    public class MiscTests
    {
        [TestMethod]
        public void Percentage_Clamp_50_to_50()
        {
            Percentage percent = 50.0m;
            Assert.IsTrue(percent == 50.0m, "A percentage of 50% did not keep its value.");
        }

        [TestMethod]
        public void Percentage_Clamp_900_to_100()
        {
            Percentage percent = 900.0m;
            Assert.IsTrue(percent == 100.0m, "Percentage values over 100.0 must be clamped to 100.0.");
        }

        [TestMethod]
        public void Percentage_Clamp_Neg50_to_0()
        {
            Percentage percent = -50.0m;
            Assert.IsTrue(percent == 0.0m, "Percentage values below 0.0 must be clamped to 0.0.");
        }

        [TestMethod]
        public void Percentage_ToString_Success()
        {
            Percentage percent = 42.0m;
            string percentString = percent.ToString();

            Assert.IsTrue(percentString == "42.0%",
                "Percentage.ToString() produced an incorrect value:\n" + percentString);
        }
    }
}
