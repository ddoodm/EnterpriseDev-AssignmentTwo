using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ENETCare.IMS.Tests
{
    [TestClass]
    public class ClientsTest
    {
        ENETCareDAO application;

        [TestInitialize]
        public void Setup()
        {
            application = new ENETCareDAO();
        }

        [TestMethod]
        public void Clients_Get_Client_By_ID_Method_Returns_Client()
        {
            int id = 2;
            Client client = application.Clients.GetClientByID(id);

            Assert.IsTrue(client.ID == id);
        }
    }
}
