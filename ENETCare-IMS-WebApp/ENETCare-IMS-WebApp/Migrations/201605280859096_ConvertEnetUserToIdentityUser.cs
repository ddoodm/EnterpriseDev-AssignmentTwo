namespace ENETCare_IMS_WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConvertEnetUserToIdentityUser : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.EnetCareUsers", newName: "AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "siteEngineerProfile_ID", "dbo.EnetCareUsers");
            DropForeignKey("dbo.InterventionApprovals", "ApprovingManagerID", "dbo.EnetCareUsers");
            DropForeignKey("dbo.InterventionApprovals", "ApprovingSiteEngineerID", "dbo.EnetCareUsers");
            DropForeignKey("dbo.Interventions", "SiteEngineer_ID", "dbo.EnetCareUsers");
            DropIndex("dbo.InterventionApprovals", new[] { "ApprovingSiteEngineerID" });
            DropIndex("dbo.InterventionApprovals", new[] { "ApprovingManagerID" });
            DropIndex("dbo.Interventions", new[] { "SiteEngineer_ID" });
            DropIndex("dbo.AspNetUsers", new[] { "siteEngineerProfile_ID" });
            RenameColumn(table: "dbo.InterventionApprovals", name: "ApprovingManagerID", newName: "ApprovingManager_Id");
            RenameColumn(table: "dbo.InterventionApprovals", name: "ApprovingSiteEngineerID", newName: "ApprovingSiteEngineer_Id");
            DropPrimaryKey("dbo.AspNetUsers");
            AddColumn("dbo.AspNetUsers", "Email", c => c.String(maxLength: 256));
            AddColumn("dbo.AspNetUsers", "EmailConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "PasswordHash", c => c.String());
            AddColumn("dbo.AspNetUsers", "SecurityStamp", c => c.String());
            AddColumn("dbo.AspNetUsers", "PhoneNumber", c => c.String());
            AddColumn("dbo.AspNetUsers", "PhoneNumberConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "TwoFactorEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "LockoutEndDateUtc", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "LockoutEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "AccessFailedCount", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "UserName", c => c.String(nullable: false, maxLength: 256));
            AlterColumn("dbo.InterventionApprovals", "ApprovingSiteEngineer_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.InterventionApprovals", "ApprovingManager_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.AspNetUsers", "Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Interventions", "SiteEngineer_Id", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.AspNetUsers", "Id");
            CreateIndex("dbo.InterventionApprovals", "ApprovingManager_Id");
            CreateIndex("dbo.InterventionApprovals", "ApprovingSiteEngineer_Id");
            CreateIndex("dbo.Interventions", "SiteEngineer_Id");
            AddForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.InterventionApprovals", "ApprovingManager_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.InterventionApprovals", "ApprovingSiteEngineer_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Interventions", "SiteEngineer_Id", "dbo.AspNetUsers", "Id");
            DropTable("dbo.AspNetUsers");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.Interventions", "SiteEngineer_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.InterventionApprovals", "ApprovingSiteEngineer_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.InterventionApprovals", "ApprovingManager_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Interventions", new[] { "SiteEngineer_Id" });
            DropIndex("dbo.InterventionApprovals", new[] { "ApprovingSiteEngineer_Id" });
            DropIndex("dbo.InterventionApprovals", new[] { "ApprovingManager_Id" });
            DropPrimaryKey("dbo.AspNetUsers");
            AlterColumn("dbo.Interventions", "SiteEngineer_Id", c => c.Int());
            AlterColumn("dbo.AspNetUsers", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.InterventionApprovals", "ApprovingManager_Id", c => c.Int());
            AlterColumn("dbo.InterventionApprovals", "ApprovingSiteEngineer_Id", c => c.Int());
            DropColumn("dbo.AspNetUsers", "UserName");
            DropColumn("dbo.AspNetUsers", "AccessFailedCount");
            DropColumn("dbo.AspNetUsers", "LockoutEnabled");
            DropColumn("dbo.AspNetUsers", "LockoutEndDateUtc");
            DropColumn("dbo.AspNetUsers", "TwoFactorEnabled");
            DropColumn("dbo.AspNetUsers", "PhoneNumberConfirmed");
            DropColumn("dbo.AspNetUsers", "PhoneNumber");
            DropColumn("dbo.AspNetUsers", "SecurityStamp");
            DropColumn("dbo.AspNetUsers", "PasswordHash");
            DropColumn("dbo.AspNetUsers", "EmailConfirmed");
            DropColumn("dbo.AspNetUsers", "Email");
            AddPrimaryKey("dbo.AspNetUsers", "ID");
            RenameColumn(table: "dbo.InterventionApprovals", name: "ApprovingSiteEngineer_Id", newName: "ApprovingSiteEngineerID");
            RenameColumn(table: "dbo.InterventionApprovals", name: "ApprovingManager_Id", newName: "ApprovingManagerID");
            CreateIndex("dbo.AspNetUsers", "siteEngineerProfile_ID");
            CreateIndex("dbo.Interventions", "SiteEngineer_ID");
            CreateIndex("dbo.InterventionApprovals", "ApprovingManagerID");
            CreateIndex("dbo.InterventionApprovals", "ApprovingSiteEngineerID");
            AddForeignKey("dbo.Interventions", "SiteEngineer_ID", "dbo.EnetCareUsers", "ID");
            AddForeignKey("dbo.InterventionApprovals", "ApprovingSiteEngineerID", "dbo.EnetCareUsers", "ID");
            AddForeignKey("dbo.InterventionApprovals", "ApprovingManagerID", "dbo.EnetCareUsers", "ID");
            AddForeignKey("dbo.AspNetUsers", "siteEngineerProfile_ID", "dbo.EnetCareUsers", "ID");
            AddForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            RenameTable(name: "dbo.AspNetUsers", newName: "EnetCareUsers");
        }
    }
}
