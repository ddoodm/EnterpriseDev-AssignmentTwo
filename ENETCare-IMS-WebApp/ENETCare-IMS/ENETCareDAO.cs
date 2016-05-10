using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

using ENETCare.IMS.Data;
using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;

namespace ENETCare.IMS
{
    /// <summary>
    /// Provides Data Access for ENETCare data sources
    /// </summary>
    public class ENETCareDAO
    {
        private static ENETCareDAO currentContext;

        private string sqlConnectionString;

        public Interventions.Interventions Interventions { get; private set; }
        public InterventionTypes InterventionTypes { get; private set; }
        public Districts Districts { get; private set; }
        public Clients Clients { get; private set; }
        public Users.Users Users { get; private set; }

        public ENETCareDAO()
        {
            sqlConnectionString = GetConnectionString();

            using (SqlConnection sqlLink = new SqlConnection(sqlConnectionString))
            {
                this.Districts = LoadDistricts(sqlLink);
                this.Clients = LoadClients(sqlLink, this.Districts);
                this.Users = LoadUsers(sqlLink, this.Districts);
                this.InterventionTypes = LoadInterventionTypes(sqlLink);
                this.Interventions = LoadInterventions(
                    sqlLink, this.InterventionTypes, this.Clients, this.Users);
            }
        }

        public static ENETCareDAO Context
        {
            // get { return (currentContext) ?? (currentContext = new ENETCareDAO()); }
            get { return (currentContext = new ENETCareDAO()); }
        }

        private string GetConnectionString()
        {
            return ConfigurationManager
                .ConnectionStrings["ENETCareDatabaseConnection"]
                .ConnectionString;
        }

        private InterventionTypes LoadInterventionTypes(SqlConnection sql)
        {
            // Select all from table, given the table name
            SqlCommand query = new SqlCommand(
                String.Format(
                    "SELECT * FROM [dbo].[{0}]",
                    DatabaseConstants.INTERVENTIONTYPES_NAME),
                sql);
            SqlDataAdapter adapter = new SqlDataAdapter(query);

            EnetCareImsDataSet dataSet = new EnetCareImsDataSet();
            adapter.Fill(dataSet, DatabaseConstants.INTERVENTIONTYPES_NAME);

            InterventionTypes types = new InterventionTypes();

            foreach (EnetCareImsDataSet.InterventionTypesRow typesRow in dataSet.InterventionTypes)
            {
                InterventionType type = new InterventionType(
                    typesRow.InterventionTypeId,
                    typesRow.Name,
                    typesRow.Cost,
                    typesRow.Labour);
                types.Add(type);
            }

            return types;
        }

        private Clients LoadClients(SqlConnection sql, Districts districts)
        {
            // Select all from table, given the table name
            SqlCommand query = new SqlCommand(
                String.Format(
                    "SELECT * FROM [dbo].[{0}]",
                    DatabaseConstants.CLIENTS_TABLE_NAME),
                sql);
            SqlDataAdapter adapter = new SqlDataAdapter(query);

            EnetCareImsDataSet dataSet = new EnetCareImsDataSet();
            adapter.Fill(dataSet, DatabaseConstants.CLIENTS_TABLE_NAME);

            Clients clients = new Clients(this);

            foreach (EnetCareImsDataSet.ClientsRow clientRow in dataSet.Clients)
            {
                District district = districts.GetDistrictByID(clientRow.DistrictId);

                Client client = new Client(
                    clientRow.ClientId, clientRow.Name, clientRow.Location, district);

                clients.Add(client);
            }

            return clients;
        }

        private Interventions.Interventions LoadInterventions(
            SqlConnection sql,
            InterventionTypes interventionTypes,
            Clients clients,
            Users.Users users)
        {
            // Select all from table, given the table name
            SqlCommand query = new SqlCommand(
                String.Format(
                    "SELECT * FROM [dbo].[{0}]",
                    DatabaseConstants.INTERVENTIONS_TABLE_NAME),
                sql);

            SqlDataAdapter adapter = new SqlDataAdapter(query);

            EnetCareImsDataSet dataSet = new EnetCareImsDataSet();
            adapter.Fill(dataSet, DatabaseConstants.INTERVENTIONS_TABLE_NAME);

            Interventions.Interventions interventions = new Interventions.Interventions(this);

            foreach (EnetCareImsDataSet.InterventionsRow row in dataSet.Interventions)
            {
                InterventionType type = interventionTypes[row.InterventionTypeId];
                Client client = clients.GetClientByID(row.ClientId);
                EnetCareUser siteEngineer = users.GetUserByID(row.ProposingEngineerId);

                if (!(siteEngineer is SiteEngineer))
                    throw new InvalidDataException(
                        String.Format("Database load error\n\nIntervention #{0} references a user who is not a Site Engineer (UserID: {1}).",
                        row.InterventionId, row.ProposingEngineerId));

                // Intervention Factory will populate with type defaults if database values are null
                decimal? labour = row.IsLabourNull() ? (decimal?)null : row.Labour;
                decimal? cost = row.IsCostNull() ? (decimal?)null : row.Cost;

                // Avoids DBNull exception
                string notes = row.IsNotesNull() ? "" : row.Notes;

                // Obtain the approval
                InterventionApproval approval =
                    LoadInterventionApproval(sql, users, row.InterventionId);

                // Obtain quality management
                InterventionQualityManagement quality =
                    LoadInterventionQualityManagement(sql, row.InterventionId);

                // Create the intervention
                Intervention intervention = 
                    Intervention.Factory.RawCreateIntervention(
                        row.InterventionId, type, client, (SiteEngineer)siteEngineer,
                        labour, cost, row.Date, notes, approval, quality
                        );

                // Link approval
                if(approval != null)
                    approval.LinkIntervention(intervention);

                interventions.Add(intervention);
            }

            return interventions;
        }

        private InterventionApproval LoadInterventionApproval(SqlConnection sql, Users.Users users, int interventionId)
        {
            // Select the approval with the given ID
            SqlCommand query = new SqlCommand(
                String.Format(
                    "SELECT * FROM [dbo].[{0}] WHERE InterventionId = @intervention_id",
                    DatabaseConstants.INTERVENTION_APPROVALS_TABLE_NAME),
                sql);
            query.Parameters.AddWithValue("@intervention_id", interventionId);

            SqlDataAdapter adapter = new SqlDataAdapter(query);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            // The query returns one table
            if (dataSet.Tables.Count != 1)
                throw new InvalidDataException();
            DataTable approvalTable = dataSet.Tables[0];

            // No approval data if table is empty
            if (approvalTable.Rows.Count < 1)
                return null;
            DataRow approvalData = approvalTable.Rows[0];

            // Cast the state ID to an InterventionApprovalState
            InterventionApprovalState state =
                (InterventionApprovalState)((int)approvalData["State"]);

            // Get the user who approved the intervention
            EnetCareUser approver =
                users.GetUserByID((int)approvalData["ApprovingUserId"]);

            // Verify that the approver is an IInterventionApprover
            if (!(approver is IInterventionApprover))
                throw new InvalidDataException("An Intervention Approval specified an incompatible approver.");

            return new InterventionApproval(state, (IInterventionApprover)approver);
        }

        private InterventionQualityManagement LoadInterventionQualityManagement(SqlConnection sql, int interventionId)
        {
            // Select the quality info with the given ID
            SqlCommand query = new SqlCommand(
                String.Format(
                    "SELECT * FROM [dbo].[{0}] WHERE InterventionId = @intervention_id",
                    DatabaseConstants.INTERVENTION_QUALITY_MANAGEMENT_TABLE_NAME),
                sql);
            query.Parameters.AddWithValue("@intervention_id", interventionId);

            SqlDataAdapter adapter = new SqlDataAdapter(query);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            // The query returns one table
            if (dataSet.Tables.Count != 1)
                throw new InvalidDataException();
            DataTable qualityTable = dataSet.Tables[0];

            // No quality data if table is empty
            if (qualityTable.Rows.Count < 1)
                return null;
            DataRow qualityData = qualityTable.Rows[0];

            return new InterventionQualityManagement(
                (decimal)qualityData["Health"], (DateTime)qualityData["LastVisit"]);
        }

        public Users.Users LoadUsers(SqlConnection sql, Districts districts)
        {
            Users.Users users = new Users.Users(this);
            users.Add(LoadSiteEngineers(sql, districts));
            users.Add(LoadManagers(sql, districts));
            users.Add(LoadAccountants(sql));
            return users;
        }

        /// <summary>
        /// Loads a collection of users of a given sub-class of User.
        /// 
        /// For example: this function may be used to load all Site Engineer users
        /// into a Users collection, when the 'tableName' is the name of the
        /// TPT (Table-Per-Type) sub-type table (ie, "Users_SiteEngineers"),
        /// and "InstantiateUser" is populated with code which returns a
        /// SiteEngineer given a row of said table.
        /// </summary>
        /// <see cref="ENETCare.IMS.ENETCareDAO.LoadSiteEngineers(SqlConnection)"/>
        /// <param name="sql">An SQL connection (initialized)</param>
        /// <param name="tableName">The name of the TPT sub-type table (ie, "Users_SiteEngineers")</param>
        /// <param name="instantiateUser">
        /// A delegate which is passed a DataRow from an inner-join of the specified
        /// sub-type table and its respective Users table row, and should return
        /// an instantiated user of the given type.
        /// </param>
        /// <returns>A Users collection, populated with all users of the specified type.</returns>
        public Users.Users LoadUsers(SqlConnection sql, string tableName, Func<DataRow, EnetCareUser> instantiateUser)
        {
            // Joins Table-Per-Type sub-class with its base class
            SqlCommand query = new SqlCommand(
                String.Format(
                    "SELECT * " +
                    "FROM   [dbo].[{0}] " +
                    "   INNER JOIN [dbo].[{1}] " +
                    "       ON [dbo].[{0}].[{2}] = [dbo].[{1}].[{2}]",
                    tableName,
                    DatabaseConstants.USERS_TABLE_NAME,
                    DatabaseConstants.USER_ID),
                sql);

            SqlDataAdapter adapter = new SqlDataAdapter(query);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            // The query returns one table
            if (dataSet.Tables.Count != 1)
                throw new InvalidDataException("An error occurred when joining database tables.");
            DataTable userTable = dataSet.Tables[0];

            // Read rows into ENETCare business objects
            Users.Users users = new Users.Users(this);
            foreach (DataRow userRow in userTable.Rows)
                users.Add(instantiateUser(userRow));

            return users;
        }

        public Users.Users LoadSiteEngineers(SqlConnection sql, Districts districts)
        {
            return LoadUsers(sql, "Users_SiteEngineers", row =>
            {
                // Find the Site Engineer's district
                District district = districts.GetDistrictByID((int)row["DistrictId"]);

                // Create the Site Engineer from table data
                return new SiteEngineer (
                    (int)row["UserId"], (string)row["Name"],district,
                    (decimal)row["MaxApprovableLabour"], (decimal)row["MaxApprovableCost"]);
            });
        }

        public Users.Users LoadManagers(SqlConnection sql, Districts districts)
        {
            return LoadUsers(sql, "Users_Managers", row =>
            {
                // Find the Manager's district
                District district = districts.GetDistrictByID((int)row["DistrictId"]);

                // Create the Manager from table data
                return new Manager(
                    (int)row["UserId"], (string)row["Name"], district,
                    (decimal)row["MaxApprovableLabour"], (decimal)row["MaxApprovableCost"]);
            });
        }

        public Users.Users LoadAccountants(SqlConnection sql)
        {
            return LoadUsers(sql, "Users_Accountants", row =>
            {
                // Create the Accountant from table data
                return new Accountant(
                    (int)row["UserId"], (string)row["Name"]);
            });
        }

        public Districts LoadDistricts(SqlConnection sql)
        {
            // Select all from table, given the table name
            SqlCommand query = new SqlCommand(
                String.Format(
                    "SELECT * FROM [dbo].[{0}]",
                    DatabaseConstants.DISTRICTS_TABLE_NAME),
                sql);

            SqlDataAdapter adapter = new SqlDataAdapter(query);
            EnetCareImsDataSet dataSet = new EnetCareImsDataSet();
            adapter.Fill(dataSet, DatabaseConstants.DISTRICTS_TABLE_NAME);

            Districts districts = new Districts(this);

            foreach (EnetCareImsDataSet.DistrictsRow districtRow in dataSet.Districts)
            {
                int id = districtRow.DistrictId;
                string name = districtRow.Name;
                District district = new District(id, name);
                districts.Add(district);
            }

            return districts;
        }

        public void Update(Intervention intervention)
        {
            using (SqlConnection sqlLink = new SqlConnection(GetConnectionString()))
            {
                sqlLink.Open();

                // Update notes
                string queryString = String.Format(
                    "UPDATE {0} SET Notes = @notes WHERE InterventionId = @interventionId;", DatabaseConstants.INTERVENTIONS_TABLE_NAME);

                SqlCommand query = new SqlCommand(queryString, sqlLink);
                query.Parameters.AddWithValue("@notes", intervention.Notes);
                query.Parameters.AddWithValue("@interventionId", intervention.ID);

                query.ExecuteNonQuery();

                // Update approval
                UpdateApproval(sqlLink, intervention);

                sqlLink.Close();
            }
        }

        public void UpdateApproval(SqlConnection sqlLink, Intervention intervention)
        {
            if (intervention.ApprovingUser == null)
                return;

            string queryString = String.Format(
                "UPDATE {0} SET State = @state, ApprovingUserId=@approverId WHERE InterventionId=@intervention \n\n" +
                "IF @@ROWCOUNT = 0\n  " +
                "INSERT INTO {0} (State, ApprovingUserId, InterventionId) VALUES (@state, @approverId, @intervention)",
                DatabaseConstants.INTERVENTION_APPROVALS_TABLE_NAME);

            SqlCommand query = new SqlCommand(queryString, sqlLink);
            query.Parameters.AddWithValue("@state", (int)intervention.ApprovalState);
            query.Parameters.AddWithValue("@approverId", intervention.ApprovingUser.ID);
            query.Parameters.AddWithValue("@intervention", intervention.ID);

            query.ExecuteNonQuery();
        }

        public void Save(Intervention intervention)
        {
            using (SqlConnection sqlLink = new SqlConnection(GetConnectionString()))
            {
                sqlLink.Open();

                // Create query, and substitute table and column names first
                string queryString = String.Format(
                    "INSERT INTO {0} ({1}) VALUES(@typeId, @client, @engineer, @date, @labour, @cost, @notes);",
                    DatabaseConstants.INTERVENTIONS_TABLE_NAME,
                    DatabaseConstants.INTERVENTIONS_COLUMN_NAMES_FOR_CREATION);

                // Substitute SQL parameters using the Parameters collection
                SqlCommand query = new SqlCommand(queryString, sqlLink);
                query.Parameters.AddWithValue("@typeId",    intervention.InterventionType.ID);
                query.Parameters.AddWithValue("@client",    intervention.Client.ID);
                query.Parameters.AddWithValue("@engineer",  intervention.SiteEngineer.ID);
                query.Parameters.AddWithValue("@date",      intervention.Date);
                query.Parameters.AddWithValue("@labour",    intervention.Labour);
                query.Parameters.AddWithValue("@cost",      intervention.Cost);

                // Permit null notes
                query.Parameters.AddWithValue("@notes",
                    ((object)intervention.Notes)?? DBNull.Value);

                query.ExecuteNonQuery();

                // Set / update approval
                if(intervention.ApprovingUser != null)
                    UpdateApproval(sqlLink, intervention);

                sqlLink.Close();
            }
        }

        public void Save(Client client)
        {
            using (SqlConnection sqlLink = new SqlConnection(GetConnectionString()))
            {
                sqlLink.Open();

                // Create query, and substitute table and column names first
                string queryString = String.Format(
                    "INSERT INTO {0} ({1}) VALUES(@name, @location, @district);",
                    DatabaseConstants.CLIENTS_TABLE_NAME,
                    DatabaseConstants.CLIENTS_COLUMN_NAMES_FOR_CREATION);

                // Substitute SQL parameters using the Parameters collection
                SqlCommand query = new SqlCommand(queryString, sqlLink);
                query.Parameters.AddWithValue("@name",      client.Name);
                query.Parameters.AddWithValue("@location",  client.Location);
                query.Parameters.AddWithValue("@district",  client.District.ID);

                query.ExecuteNonQuery();
                sqlLink.Close();
            }
        }

        public void UpdateSiteEngineer(SiteEngineer engineer)
        {
            using (SqlConnection sqlLink = new SqlConnection(GetConnectionString()))
            {
                sqlLink.Open();

                string queryString = String.Format(
                    "UPDATE {0} SET DistrictId = @districtID, MaxApprovableLabour = @labour, MaxApprovableCost = @cost WHERE UserId = @userID;", DatabaseConstants.SITE_ENGINEERS_TABLE_NAME);

                SqlCommand query = new SqlCommand(queryString, sqlLink);
                query.Parameters.AddWithValue("@districtID", engineer.District.ID);
                query.Parameters.AddWithValue("@labour", engineer.MaxApprovableLabour);
                query.Parameters.AddWithValue("@cost", engineer.MaxApprovableCost);
                query.Parameters.AddWithValue("@userID", engineer.ID);

                query.ExecuteNonQuery();
                sqlLink.Close();
            }
        }

        public void UpdateManager(Manager manager)
        {
            using (SqlConnection sqlLink = new SqlConnection(GetConnectionString()))
            {
                sqlLink.Open();

                string queryString = String.Format(
                    "UPDATE {0} SET DistrictId = @districtID, MaxApprovableLabour = @labour, MaxApprovableCost = @cost WHERE UserId = @userID;", DatabaseConstants.MANAGERS_TABLE_NAME);

                SqlCommand query = new SqlCommand(queryString, sqlLink);
                query.Parameters.AddWithValue("@districtID", manager.District.ID);
                query.Parameters.AddWithValue("@labour", manager.MaxApprovableLabour);
                query.Parameters.AddWithValue("@cost", manager.MaxApprovableCost);
                query.Parameters.AddWithValue("@userID", manager.ID);

                query.ExecuteNonQuery();
                sqlLink.Close();
            }
        }

        public void RefreshUsers()
        {
            using (SqlConnection sqlLink = new SqlConnection(sqlConnectionString))
            {
                this.Users = LoadUsers(sqlLink, this.Districts);
            }
        }

        //Beginnings of Quality Management updater - dependant on Quality Loader
        public void UpdateQuality(InterventionQualityManagement quality)
        {
            using (SqlConnection sqlLink = new SqlConnection(GetConnectionString()))
            {
                sqlLink.Open();

                string queryString = String.Format(
                    "UPDATE {0} SET Health = @health WHERE LastVisit = @visit;", DatabaseConstants.INTERVENTION_QUALITY_MANAGEMENT_TABLE_NAME);

                SqlCommand query = new SqlCommand(queryString, sqlLink);
                query.Parameters.AddWithValue("@health", quality.Health);
                query.Parameters.AddWithValue("@visit", quality.LastVisit);

                query.ExecuteNonQuery();
                sqlLink.Close();
            }
        }
    }
}
