using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS.Data
{
    public class DatabaseConstants
    {
        public const string CLIENTS_TABLE_NAME = "Clients";
        public const string CLIENTS_COLUMN_NAMES_FOR_CREATION = "Name, Location, DistrictId";

        public const string INTERVENTIONS_TABLE_NAME = "Interventions";
        public const string INTERVENTIONS_COLUMN_NAMES = "InterventionID, InterventionTypeID, ClientID, ProposingEngineerID, Date, Labour, Cost, Notes";
        public const string INTERVENTIONS_COLUMN_NAMES_FOR_CREATION = "InterventionTypeID, ClientID, ProposingEngineerID, Date, Labour, Cost, Notes";

        public const string INTERVENTIONTYPES_NAME = "InterventionTypes";
        public const string INTERVENTIONTYPES_COLUMN_NAMES = "InterventionTypeId, Name, Cost, Labour";
        public const string INTERVENTIONTYPES_COLUMN_NAMES_FOR_CREATION = "Name, Cost, Labour";

        public const string INTERVENTION_APPROVALS_TABLE_NAME = "InterventionApprovals";

        public const string INTERVENTION_QUALITY_MANAGEMENT_TABLE_NAME = "InterventionQualityManagement";
        public const string INTERVENTION_QUALITY_MANAGEMENT_COLUMN_NAMES = "InterventionQualityID, InterventionID, Health, LastVisit";
        public const string INTERVENTION_QUALITY_MANAGEMENT_COLUMN_NAMES_FOR_CREATION = "InterventionID, Health, LastVisit";

        public const string USERS_TABLE_NAME = "Users";
        public const string SITE_ENGINEERS_TABLE_NAME = "Users_SiteEngineers";
        public const string MANAGERS_TABLE_NAME = "Users_Managers";
        public const string MANAGERS_AND_SITE_ENGINEERS_COLUMN_NAMES = "UserId, DistrictId, MaxApprovableLabour, MaxApprovableCost";
        public const string MANAGERS_AND_SITE_ENGINEERS_COLUMN_NAMES_FOR_CREATION = "DistrictId, MaxApprovableLabour, MaxApprovableCost";
        public const string USERS_COLUMN_NAMES_FOR_CREATION = "Name, UserName, PlaintextPassword, Password";

        public const string DISTRICTS_TABLE_NAME = "Districts";
        public const string DISTRICTS_COLUMNS_FOR_CREATION = "Name";
        
        //public const string USERS_MANAGERS_JOIN_SQL = "SELECT Users_Managers.UserId, Users.Name, Users.PlaintextPassword, Users.Password, Users_Managers.DistrictId, Users_Managers.MaxApprovableLabour, Users_Managers.MaxApprovableCost FROM Users INNER JOIN Users_Managers ON Users.UserID = Users_Managers.UserID";
        //public const string USERS_SITEENGINEERS_JOIN_SQL = "SELECT Users_SiteEngineers.UserId, Users.Name, Users.PlaintextPassword, Users.Password, Users_SiteEngineers.DistrictId, Users_SiteEngineers.MaxApprovableLabour, Users_SiteEngineers.MaxApprovableCost FROM Users INNER JOIN Users_SiteEngineers ON Users.UserID = Users_SiteEngineers.UserID";

        public const string USERS_JOIN_TABLE_NAME = "UserJoinTable";
        public const string USER_ID = "UserId";


    }
}
