using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ENETCare.IMS.MailService;
using ENETCare.IMS;
using ENETCare.IMS.Users;
using ENETCare.IMS.Interventions;

namespace ENETCare_IMS_WebApp.Tests.MailerTests
{
    [TestClass]
    public class EnetCareMailerTests
    {
        [TestMethod]
        public void MailService_ReadLayoutFile_Success()
        {
            var district = new District("Test District");
            var client = new Client("Test Client", "Test Location", district);
            var engineer = new SiteEngineer("Test Engineer", "test@mail.com", "1234Abcd!", district, 1000, 1000);
            var intervention = Intervention.Factory.CreateIntervention(
                new InterventionType("Example Intervention", 40, 2),
                client, engineer);

            EnetCareMailer.NotifyEngineerOfStateChange(intervention);
        }
    }
}
