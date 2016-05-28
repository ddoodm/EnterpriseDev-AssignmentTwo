namespace ENETCare_IMS_WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DistrictID = c.Int(nullable: false),
                        Location = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Districts", t => t.DistrictID, cascadeDelete: true)
                .Index(t => t.DistrictID);
            
            CreateTable(
                "dbo.Districts",
                c => new
                    {
                        DistrictID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.DistrictID);
            
            CreateTable(
                "dbo.InterventionApprovals",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        state_CurrentState = c.Int(nullable: false),
                        ApprovingSiteEngineerID = c.Int(),
                        ApprovingManagerID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.EnetCareUsers", t => t.ApprovingManagerID)
                .ForeignKey("dbo.EnetCareUsers", t => t.ApprovingSiteEngineerID)
                .Index(t => t.ApprovingSiteEngineerID)
                .Index(t => t.ApprovingManagerID);
            
            CreateTable(
                "dbo.EnetCareUsers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        MaxApprovableLabour = c.Decimal(precision: 18, scale: 2),
                        MaxApprovableCost = c.Decimal(precision: 18, scale: 2),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        District_DistrictID = c.Int(),
                        District_DistrictID1 = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Districts", t => t.District_DistrictID)
                .ForeignKey("dbo.Districts", t => t.District_DistrictID1)
                .Index(t => t.District_DistrictID)
                .Index(t => t.District_DistrictID1);
            
            CreateTable(
                "dbo.Interventions",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Labour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Notes = c.String(),
                        Client_ID = c.Int(),
                        InterventionType_ID = c.Int(),
                        Quality_ID = c.Int(),
                        SiteEngineer_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.InterventionApprovals", t => t.ID)
                .ForeignKey("dbo.Clients", t => t.Client_ID)
                .ForeignKey("dbo.InterventionTypes", t => t.InterventionType_ID)
                .ForeignKey("dbo.InterventionQualityManagement", t => t.Quality_ID)
                .ForeignKey("dbo.EnetCareUsers", t => t.SiteEngineer_ID)
                .Index(t => t.ID)
                .Index(t => t.Client_ID)
                .Index(t => t.InterventionType_ID)
                .Index(t => t.Quality_ID)
                .Index(t => t.SiteEngineer_ID);
            
            CreateTable(
                "dbo.InterventionTypes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Labour = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.InterventionQualityManagement",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Health_Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LastVisit = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        siteEngineerProfile_ID = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EnetCareUsers", t => t.siteEngineerProfile_ID)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.siteEngineerProfile_ID);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "siteEngineerProfile_ID", "dbo.EnetCareUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Interventions", "SiteEngineer_ID", "dbo.EnetCareUsers");
            DropForeignKey("dbo.Interventions", "Quality_ID", "dbo.InterventionQualityManagement");
            DropForeignKey("dbo.Interventions", "InterventionType_ID", "dbo.InterventionTypes");
            DropForeignKey("dbo.Interventions", "Client_ID", "dbo.Clients");
            DropForeignKey("dbo.Interventions", "ID", "dbo.InterventionApprovals");
            DropForeignKey("dbo.InterventionApprovals", "ApprovingSiteEngineerID", "dbo.EnetCareUsers");
            DropForeignKey("dbo.EnetCareUsers", "District_DistrictID1", "dbo.Districts");
            DropForeignKey("dbo.InterventionApprovals", "ApprovingManagerID", "dbo.EnetCareUsers");
            DropForeignKey("dbo.EnetCareUsers", "District_DistrictID", "dbo.Districts");
            DropForeignKey("dbo.Clients", "DistrictID", "dbo.Districts");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "siteEngineerProfile_ID" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Interventions", new[] { "SiteEngineer_ID" });
            DropIndex("dbo.Interventions", new[] { "Quality_ID" });
            DropIndex("dbo.Interventions", new[] { "InterventionType_ID" });
            DropIndex("dbo.Interventions", new[] { "Client_ID" });
            DropIndex("dbo.Interventions", new[] { "ID" });
            DropIndex("dbo.EnetCareUsers", new[] { "District_DistrictID1" });
            DropIndex("dbo.EnetCareUsers", new[] { "District_DistrictID" });
            DropIndex("dbo.InterventionApprovals", new[] { "ApprovingManagerID" });
            DropIndex("dbo.InterventionApprovals", new[] { "ApprovingSiteEngineerID" });
            DropIndex("dbo.Clients", new[] { "DistrictID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.InterventionQualityManagement");
            DropTable("dbo.InterventionTypes");
            DropTable("dbo.Interventions");
            DropTable("dbo.EnetCareUsers");
            DropTable("dbo.InterventionApprovals");
            DropTable("dbo.Districts");
            DropTable("dbo.Clients");
        }
    }
}
