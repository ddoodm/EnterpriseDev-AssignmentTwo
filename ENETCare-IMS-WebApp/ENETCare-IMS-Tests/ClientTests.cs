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
        [TestInitialize]
        public void Setup()
        {
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

        [TestMethod]
        public void Clients_Save_Client_To_Repo_Test_Getting_Nth_Client_By_Name()
        {
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                ClientRepo testRepo = new ClientRepo(db);
                Client storedClient = new Client("testName", "testLocation", new District("testDistrict"));
                testRepo.Save(storedClient);

                Assert.IsTrue(testRepo.GetNthClient(0).Name == "testName");
            }
        }
    }
}

