using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ENETCare.IMS;
using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;
using ENETCare.IMS.Data.DataAccess;

namespace ENETCare.IMS.Tests
{
    [TestClass]
    public class ClientTests
    {
        private ClientRepo clientRepo;

        [TestInitialize]
        public void Setup()
        {
            clientRepo = ClientRepo.New;
        }

        [TestMethod]
        public void Clients_Get_Client_By_ID_Method_Returns_Client()
        {
            /*
            int id = 2;
            Client client = application.Clients.GetClientByID(id);

            Assert.IsTrue(client.ID == id);
            */

            // With EF, we cannot guarantee any primary key, so this test no longer makes sense
            Assert.Fail();
        }
    }
}
