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

using Moq;
using Moq.Protected;

namespace ENETCare_IMS_WebApp.Tests.MailerTests
{
    [TestClass]
    public class EnetCareMailerTests
    {
        private Intervention NewTestIntervention
        {
            get
            {
                var district = new District("Test District");
                var client = new Client("Test Client", "Test Location", district);
                var engineer = new SiteEngineer("Test Engineer", "test@mail.com", "1234Abcd!", district, 1000, 1000);
                var intervention = Intervention.Factory.CreateIntervention(
                    new InterventionType("Example Intervention", 40, 2),
                    client, engineer);
                return intervention;
            }
        }

        private EnetCareMailer MailerWithFakeDelivery(IMailDeliverer fakeDeliverer)
        {
            var mockMailer = new Mock<EnetCareMailer>();
            mockMailer.Protected()
                .Setup<IMailDeliverer>("GetMailDeliverer")
                .Returns(fakeDeliverer);

            return mockMailer.Object;
        }

        [TestMethod]
        public void MailService_Message_Mentions_ApprovalState_And_Client_Details_Success()
        {
            var intervention = NewTestIntervention;

            // A mail deliverer that does nothing but test
            var mockMailDeliverer = new Mock<IMailDeliverer>();
            mockMailDeliverer.Setup(d => d.SendMail(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string to, string from, string subject, string htmlBody) =>
                {
                    // Mailer will build a message and reach here (to deliver the message)
                    // Here we check that the message makes mention of the state and the client

                    bool containsApproval = htmlBody.Contains(intervention.ApprovalState.ToString());
                    bool containsClient = htmlBody.Contains(intervention.Client.DescriptiveName);

                    if (!containsApproval || !containsClient)
                        Assert.Fail("The approval notification E-Mail did not mention the approval state or the client.");
                });

            // Mailer which uses the fake deliverer
            MailerWithFakeDelivery(mockMailDeliverer.Object)
                .NotifyEngineerOfStateChange(intervention);
        }

        [TestMethod]
        public void MailService_Message_To_Intervention_Proposer()
        {
            var intervention = NewTestIntervention;

            // A mail deliverer that does nothing but test
            var mockMailDeliverer = new Mock<IMailDeliverer>();
            mockMailDeliverer.Setup(d => d.SendMail(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string to, string from, string subject, string htmlBody) =>
                {
                    Assert.IsTrue(to == intervention.SiteEngineer.Email);
                });

            // Mailer which uses the fake deliverer
            MailerWithFakeDelivery(mockMailDeliverer.Object)
                .NotifyEngineerOfStateChange(intervention);
        }
    }
}
