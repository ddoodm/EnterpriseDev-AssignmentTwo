using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ENETCare.IMS;
using ENETCare.IMS.Interventions;
using ENETCare.IMS.Users;

namespace ENETCare.IMS.Tests
{
    [TestClass]
    public class InterventionsTests
    {
        #region Shared Test Data
        private Client testClient;
        private SiteEngineer testEngineer;

        private const uint NUM_TEST_DISTRICTS = 3;
        private District testDistrictA, testDistrictB, testDistrictC;

        private ENETCareDAO application;
        private Clients clients;
        private Districts districts;
        private InterventionTypes interventionTypes;
        #endregion

        #region Helper Data Creation Functions
        private InterventionType CreateTestInterventionType()
        {
            Assert.IsTrue(interventionTypes.Count >= 1, "There are no Intervention Types");

            return interventionTypes[1];
        }

        private Client CreateTestClient()
        {
            return new Client
                (10, "Foobar Family", "1 Madeup Lane, Fakeland", testDistrictA);
        }

        private void CreateTestDistricts()
        {
            Assert.IsTrue(districts.Count >= NUM_TEST_DISTRICTS,
                "There are not enough districts for testing.");

            testDistrictA = districts.GetDistrictByID(1);
            testDistrictB = districts.GetDistrictByID(2);
            testDistrictC = districts.GetDistrictByID(3);
        }

        private SiteEngineer CreateTestSiteEngineer()
        {
            return new SiteEngineer
                (1, "Robert Markson",
                testDistrictA, 48, 100000);
        }

        private SiteEngineer CreateTestSiteEngineerNoAutoApprove(InterventionType interventionType)
        {
            // Test engineer can not auto-approve
            return new SiteEngineer
                (1, "Markus Markson", testDistrictA,
                interventionType.Labour - 1,
                interventionType.Cost - 100);
        }

        private Intervention CreateTestIntervention(SiteEngineer testEngineer)
        {
            InterventionType interventionType = CreateTestInterventionType();

            return Intervention.Factory.CreateIntervention
                (0, interventionType, testClient, testEngineer);
        }
        private Intervention CreateCancelledIntervention(SiteEngineer testEngineer)
        {
            InterventionType interventionType = CreateTestInterventionType();

            Intervention i =  Intervention.Factory.CreateIntervention
                (0, interventionType, testClient, testEngineer);
            i.Cancel(testEngineer);

            return i;

        }

        #endregion

        [TestInitialize]
        public void Setup()
        {
            application = new ENETCareDAO();
            districts = application.Districts;
            clients = application.Clients;
            interventionTypes = application.InterventionTypes;

            CreateTestDistricts();
            testClient = CreateTestClient();
            testEngineer = CreateTestSiteEngineer();
        }

        #region Creation Tests
        /// <summary>
        /// Create an Intervention, given all parameters
        /// </summary>
        [TestMethod]
        public void Intervention_Create_All_Data_Supplied_Success()
        {
            InterventionType interventionType = CreateTestInterventionType();

            Intervention intervention = Intervention.Factory.CreateIntervention
                (0, interventionType, testClient, testEngineer,
                2, 400, DateTime.Now.AddDays(10));

            Assert.IsNotNull(intervention);
        }

        /// <summary>
        /// Create an Intervention, given only its type, siteEngineer and client
        /// </summary>
        [TestMethod]
        public void Intervention_Create_No_Data_Supplied_Success()
        {
            Intervention intervention = CreateTestIntervention(testEngineer);

            Assert.IsNotNull(intervention);
        }

        /// <summary>
        /// Instantiates an Intervention where the Client and Site Engineer
        /// Districts do not match. This operation should not be permitted
        /// by the User Interface, and should throw an Argument Exception
        /// if encountered. 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Intervention_Create_District_Discrepancy_Failure()
        {
            InterventionType interventionType = CreateTestInterventionType();

            // Create a new Engineer who does not service 'testDistrictA'
            SiteEngineer remoteEngineer = new SiteEngineer
                (1, "Markus Markson",
                testDistrictB, interventionType.Labour + 1, interventionType.Cost + 100);

            // Expected argument exception:
            Intervention intervention = Intervention.Factory.CreateIntervention
                (0, interventionType, testClient, remoteEngineer);

            Assert.Fail("Instantiation of Intervention with mismatched districts should result in an ArgumentException");
        }
        #endregion

        #region Approval Tests
        /// <summary>
        /// Creates an Intervention and attempts to approve it
        /// using the Engineer who proposed the Intervention.
        /// </summary>
        [TestMethod]
        public void Intervention_Approve_By_Engineer_Success()
        {
            InterventionType interventionType = CreateTestInterventionType();

            // Create a new Engineer that can approve the new intervention
            SiteEngineer testEngineer = new SiteEngineer
                (1, "Markus Markson",
                testDistrictA, interventionType.Labour + 1, interventionType.Cost + 100);

            Intervention intervention = Intervention.Factory.CreateIntervention
                (0, interventionType, testClient, testEngineer);

            // Attempt to approve the intervention by the Engineer who proposed it
            intervention.Approve(testEngineer);

            // Check that the state changed
            if (intervention.ApprovalState != InterventionApprovalState.Approved)
                Assert.Fail("Intervention.Approve() executed successfully, but the Intervention was not approved.");
        }

        /// <summary>
        /// Creates an Intervention and attempts to approve it by
        /// a Manager who operates in the same District as the Intervention.
        /// </summary>
        [TestMethod]
        public void Intervention_Approve_By_Manager_Success()
        {
            InterventionType interventionType = CreateTestInterventionType();

            // Create a new Engineer (make aut-approve impossible)
            SiteEngineer testEngineer = new SiteEngineer
                (1, "Markus Markson",
                testDistrictA, interventionType.Labour - 1, interventionType.Cost - 100);

            // (Will not auto-approve)
            Intervention intervention = Intervention.Factory.CreateIntervention
                (0, interventionType, testClient, testEngineer);

            // Create a Manager who operates in the same District as the Intervention
            Manager testManager = new Manager
                (2, "Bob Bobson",
                intervention.District, interventionType.Labour + 1, interventionType.Cost + 100);

            // Attempt to approve the intervention by a Manager of the same district
            intervention.Approve(testManager);

            // Check that the state changed
            if (intervention.ApprovalState != InterventionApprovalState.Approved)
                Assert.Fail("Intervention.Approve() executed successfully, but the Intervention was not approved.");
        }

        /// <summary>
        /// Attempts to approve an Intervention by an Engineer other than
        /// the Engineer who made the proposition. 
        /// 
        /// Expects an Invalid Operation Exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Intervention_Approve_By_Distinct_Engineer_Failure()
        {
            Intervention intervention = CreateTestIntervention(testEngineer);

            // Create a new Engineer who would otherwise be permitted to approve the Intervention
            SiteEngineer newEngineer = new SiteEngineer
                (2, "Markus Markson",
                intervention.District, intervention.Labour + 1, intervention.Cost + 100);

            // Attempt to approve the intervention by an Engineer who did not propose it
            // Should throw an Invalid Operation Exception
            intervention.Approve(newEngineer);

            Assert.Fail("A Site Engineer was permitted to approve an intervention that they did not create.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Intervention_Approve_By_Foreign_Manager_Failure()
        {
            InterventionType interventionType = CreateTestInterventionType();

            // Create a new Engineer
            SiteEngineer testEngineer = new SiteEngineer
                (1, "Markus Markson",
                testDistrictA, interventionType.Labour + 1, interventionType.Cost + 100);

            Intervention intervention = Intervention.Factory.CreateIntervention
                (0, interventionType, testClient, testEngineer);

            // Create a Manager who does not operate in the same district as the Intervention
            Manager testManager = new Manager
                (2, "Bob Bobson",
                testDistrictB, interventionType.Labour + 1, interventionType.Cost + 100);

            // Attempt to approve the intervention by a Manager who does not operate in the same District
            intervention.Approve(testManager);

            Assert.Fail("A Manager was permitted to approve an Intervention that is proposed for a District that the Manager does not operate in.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Intervention_Approve_With_Actual_Cost_Greater_Than_Approver_Max_Cost_Failure()
        {
            InterventionType interventionType = CreateTestInterventionType();

            SiteEngineer testEngineer = new SiteEngineer
                (1, "Markus Markson",
                testDistrictA,
                interventionType.Labour,
                interventionType.Cost);         // Capable of approving the DEFAULT value

            Intervention intervention = Intervention.Factory.CreateIntervention
                (1, interventionType, testClient, testEngineer,
                interventionType.Labour,
                interventionType.Cost + 100,    // Not capable of approving the ACTUAL value
                DateTime.Now.AddDays(1));

            // Engineer attempts to approve an Intervention with a
            // greater cost then their maximum permissable cost.
            // Should throw an invalid operation exception
            intervention.Approve(testEngineer);

            // Error on success
            Assert.Fail("An engineer was permitted to approve an intervention whose actual cost exceeds the engineer's max approvable cost.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Intervention_Approve_With_Default_Cost_Greater_Than_Approver_Max_Cost_Failure()
        {
            InterventionType interventionType = CreateTestInterventionType();

            SiteEngineer testEngineer = new SiteEngineer
                (1, "Markus Markson",
                testDistrictA,
                interventionType.Labour + 1,
                interventionType.Cost - 100);   // Not capable of approving the DEFAULT value

            Intervention intervention = Intervention.Factory.CreateIntervention
                (1, interventionType, testClient, testEngineer,
                interventionType.Labour,
                1.0m,                           // Engineer sets the cost at $1.00
                DateTime.Now.AddDays(1));

            // Engineer attempts to approve an Intervention with a fabricated cost
            // Should throw an invalid operation exception
            intervention.Approve(testEngineer);

            // Error on success
            Assert.Fail("An engineer was permitted to approve an intervention whose default cost exceeds the engineer's max approvable cost.");
        }
        #endregion

        #region Approval State Change Tests
        [TestMethod]
        public void Intervention_Initial_State_Is_Proposed_No_AutoApprove_Success()
        {
            var interventionType = CreateTestInterventionType();
            SiteEngineer testEngineer = CreateTestSiteEngineerNoAutoApprove(interventionType);

            // Create the Intervention
            Intervention intervention = CreateTestIntervention(testEngineer);

            // Check that the initial state is 'Proposed'
            if (intervention.ApprovalState != InterventionApprovalState.Proposed)
                Assert.Fail("An Intervention was created with an initial state other than 'Proposed'");
        }

        [TestMethod]
        public void Intervention_State_Change_Proposed_To_Approved_Success()
        {
            // Create the Intervention
            SiteEngineer testEngineer = CreateTestSiteEngineer();
            Intervention intervention = CreateTestIntervention(testEngineer);

            // Try to approve
            intervention.Approve(testEngineer);

            // Check that the state changed
            if (intervention.ApprovalState != InterventionApprovalState.Approved)
                Assert.Fail("Intervention.Approve() executed successfully, but the Intervention was not approved.");
        }

        [TestMethod]
        public void Intervention_State_Change_Proposed_To_Cancelled_Success()
        {
            // Create the Intervention
            SiteEngineer testEngineer = CreateTestSiteEngineer();
            Intervention intervention = CreateTestIntervention(testEngineer);

            // Try to cancel
            intervention.Cancel(testEngineer);

            // Check that the state changed
            if (intervention.ApprovalState != InterventionApprovalState.Cancelled)
                Assert.Fail("Intervention.Cancel() executed successfully, but the Intervention was not cancelled.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Intervention_State_Change_Proposed_To_Completed_Failure()
        {
            // Do not allow auto-approve on the intervention
            var interventionType = CreateTestInterventionType();
            SiteEngineer testEngineer = CreateTestSiteEngineerNoAutoApprove(interventionType);
            Intervention intervention = CreateTestIntervention(testEngineer);

            // Try to complete
            // Should throw an InvalidOperationException,
            // because the Intervention has not been approved yet.
            intervention.Complete(testEngineer);

            Assert.Fail("An Intervention was permitted to Complete when it was in its Proposed state.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Intervention_State_Change_Cancelled_To_Approved_Failure()
        {
            // Create the Intervention
            SiteEngineer testEngineer = CreateTestSiteEngineer();
            Intervention intervention = CreateTestIntervention(testEngineer);

            // Cancel the Intervention
            intervention.Cancel(testEngineer);

            // Try to approve
            // Should throw an InvalidOperationException
            intervention.Approve(testEngineer);

            Assert.Fail("An Intervention was permitted to be approved when it was in its Cancelled state.");
        }

        [TestMethod]
        public void Intervention_State_Change_Approved_To_Completed_Success()
        {
            // Create the Intervention
            SiteEngineer testEngineer = CreateTestSiteEngineer();
            Intervention intervention = CreateTestIntervention(testEngineer);

            // Approve the Intervention (should work)
            intervention.Approve(testEngineer);

            // Try to complete the Intervention
            intervention.Complete(testEngineer);

            if (intervention.ApprovalState != InterventionApprovalState.Completed)
                Assert.Fail("Intervention failed to transition from 'Approved' to 'Completed'.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Intervention_State_Change_By_Manager_Approved_To_Cancelled_Failure()
        {
            // Do not allow auto-approve
            var interventionType = CreateTestInterventionType();
            SiteEngineer testEngineer = CreateTestSiteEngineerNoAutoApprove(interventionType);
            Intervention intervention = CreateTestIntervention(testEngineer);

            // Create a Manager
            Manager testManager = new Manager
                (2, "William Williams", intervention.District,
                intervention.Labour + 1000, intervention.Cost + 1000);

            // Approve the Intervention using the manager (should work)
            try { intervention.Approve(testManager); }
            catch (Exception e) { Assert.Fail(e.Message); }

            // Try have the manager cancel the approved intervention
            // (should throw an Invalid Operation exception)
            intervention.Cancel(testManager);
        }

        [TestMethod]
        public void Intervention_AutoApprove_Success()
        {
            // Create the Intervention
            SiteEngineer testEngineer = CreateTestSiteEngineer();
            Intervention intervention = CreateTestIntervention(testEngineer);

            // Check that the intervention has been approved
            if (intervention.ApprovalState != InterventionApprovalState.Approved)
                Assert.Fail("Intervention was not auto-approved.");
        }

        [TestMethod]
        public void Intervention_AutoApprove_Fail()
        {
            // Create an engineer who is not permitted to approve this type
            var interventionType = CreateTestInterventionType();
            SiteEngineer testEngineer = CreateTestSiteEngineerNoAutoApprove(interventionType);
            Intervention intervention = CreateTestIntervention(testEngineer);

            // Check that the intervention has not been approved
            if (intervention.ApprovalState != InterventionApprovalState.Proposed)
                Assert.Fail("Intervention was auto-approved by an ineligible Site Engineer");
        }
        #endregion

        [TestMethod]
        public void Intervention_Does_Not_Appear_In_Interventions()
        {
            // Create an engineer who is not permitted to approve this type
            Intervention intervention = CreateCancelledIntervention(testEngineer);

            ENETCareDAO dao = new ENETCareDAO();
            dao.Interventions.Add(intervention);

            // Check that the intervention has not been approved
            if(dao.Interventions.GetInterventions().Count == 0)
                Assert.Fail("Cancelled intervention still appears in Interventions");

            if(dao.Interventions.GetInterventions().Find(i => i.ID == intervention.ID) != null)
                Assert.Fail("Cancelled intervention still appears in Interventions");
        }
    }
}
