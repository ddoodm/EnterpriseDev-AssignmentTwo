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

        /// <summary>
        /// Tests the correct client is retrieved by the given ID
        /// </summary>
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

        /// <summary>
        /// Tests retrieving a client by index
        /// </summary>
        [TestMethod]
        public void Clients_Save_Client_To_Repo_Test_Getting_Nth_Client_By_Name()
        {
            //this doesn't seem to work
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                ClientRepo testRepo = new ClientRepo(db);
                Client storedClient = new Client("testName", "testLocation", new District("testDistrict"));
                testRepo.Save(storedClient);

                //assumes 5 entries already
                Assert.IsTrue(testRepo.GetNthClient(6).Name == "testName");
            }
        }

        /// <summary>
        /// Tests adding a client to a list of clients then check the name persists
        /// </summary>
        [TestMethod]
        public void Clients_Add_Client_Then_Retrieve_By_ID_Comparing_Name()
        {
            Clients clients = new Clients();
            Client client = new Client("testName", "testLocation", new District("testDistrict"));

            clients.Add(client);

            Assert.IsTrue(clients[0].Name == "testName");
        }

        /// <summary>
        /// Tests creating a client by CreateClient and testing the name persists
        /// </summary>
        [TestMethod]
        public void Clients_Create_Client()
        {
            Clients clients = new Clients();

            Client client = clients.CreateClient("testName", "testLocation", new District("testDistrict"));

            Assert.IsTrue(client.Name == "testName");
        }

        /// <summary>
        /// Tests filtering a list of clients by name. Expects the correct list size and name
        /// </summary>
        [TestMethod]
        public void Clients_Filter_By_Name()
        {
            Clients clients = new Clients();
            Clients clientsFiltered;

            clients.CreateClient("testName1", "testLocation1", new District("testDistrict"));
            clients.CreateClient("testName2", "testLocation2", new District("testDistrict"));
            clients.CreateClient("testName3", "testLocation3", new District("testDistrict"));
            clients.CreateClient("testName2", "testLocation4", new District("testDistrict"));

            clientsFiltered = clients.FilterByName("testName2");
            
            Assert.IsTrue(clientsFiltered.Count==2 && clientsFiltered[1].Name == "testName2");
        }

        /// <summary>
        /// Tests filtering a list of clients by district name. Expects the correct list size and district name
        /// </summary>
        [TestMethod]
        public void Clients_Filter_By_District()
        {
            Clients clients = new Clients();
            Clients clientsFiltered;

            District district = new District("testDistrict2");

            clients.CreateClient("testName1", "testLocation1", new District("testDistrict1"));
            clients.CreateClient("testName2", "testLocation2", district);
            clients.CreateClient("testName3", "testLocation3", new District("testDistrict3"));
            clients.CreateClient("testName2", "testLocation4", district);

            //this currently returns all 4 entries (expected: 2). consider revision
            clientsFiltered = clients.FilterByDistrict(district);

            Assert.IsTrue(clientsFiltered.Count == 2 && clientsFiltered[1].District.Name == "testDistrict2");
        }

        /// <summary>
        /// Tests filtering a list of clients by name. Expects the correct list size
        /// </summary>
        [TestMethod]
        public void Clients_Filter_By_Count_Alone()
        {
            Clients clients = new Clients();
            Clients clientsFiltered;

            clients.CreateClient("testName1", "testLocation1", new District("testDistrict"));
            clients.CreateClient("testName2", "testLocation2", new District("testDistrict"));
            clients.CreateClient("testName3", "testLocation3", new District("testDistrict"));
            clients.CreateClient("testName2", "testLocation4", new District("testDistrict"));

            clientsFiltered = clients.FilterByName("testName2");

            Assert.IsTrue(clientsFiltered.Count == 2);
        }
    }
}

