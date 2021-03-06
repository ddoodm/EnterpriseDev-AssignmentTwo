﻿using System;
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

