using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ENETCare.IMS.Tests
{
    [TestClass]
    public class DatabaseTests
    {
        private string connectionString;

        /// <summary>
        /// Sets the path of the Data directory for
        /// the Connection String to correctly attach
        /// the MDF database to the server.
        /// </summary>
        [AssemblyInitialize]
        public static void SetupDataDirectory(TestContext context)
        {
            string path = Path.GetFullPath(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                @"..\..\..\ENETCare-IMS-Data"));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
        }

        [TestInitialize]
        public void Setup()
        {
            this.connectionString = ConfigurationManager
                .ConnectionStrings["ENETCareDatabaseConnection"]
                .ConnectionString;
        }

        [TestMethod]
        public void Connection_OpenClose_Success()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            conn.Close();
        }

        [TestMethod]
        public void Database_Client_1_Is_JohnSmith_Success()
        {
            using (SqlConnection c = new SqlConnection(connectionString))
            {
                string query = "SELECT Name FROM Clients WHERE ClientId = 1";
                SqlDataAdapter adapter =
                    new SqlDataAdapter(query, c);

                DataSet result = new DataSet();
                adapter.Fill(result);

                string clientName =
                    result.Tables[0].Rows[0][0].ToString();

                Assert.AreEqual(clientName, "John Smith");
            }
        }
    }
}
