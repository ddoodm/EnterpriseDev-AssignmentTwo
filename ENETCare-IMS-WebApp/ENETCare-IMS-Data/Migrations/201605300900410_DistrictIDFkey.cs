namespace ENETCare.IMS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DistrictIDFkey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "District_DistrictID", "dbo.Districts");
            DropForeignKey("dbo.AspNetUsers", "District_DistrictID1", "dbo.Districts");
            DropIndex("dbo.AspNetUsers", new[] { "District_DistrictID" });
            DropIndex("dbo.AspNetUsers", new[] { "District_DistrictID1" });
            AddColumn("dbo.AspNetUsers", "DistrictID", c => c.Int());
            DropColumn("dbo.AspNetUsers", "District_DistrictID");
            DropColumn("dbo.AspNetUsers", "District_DistrictID1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "District_DistrictID1", c => c.Int());
            AddColumn("dbo.AspNetUsers", "District_DistrictID", c => c.Int());
            DropColumn("dbo.AspNetUsers", "DistrictID");
            CreateIndex("dbo.AspNetUsers", "District_DistrictID1");
            CreateIndex("dbo.AspNetUsers", "District_DistrictID");
            AddForeignKey("dbo.AspNetUsers", "District_DistrictID1", "dbo.Districts", "DistrictID");
            AddForeignKey("dbo.AspNetUsers", "District_DistrictID", "dbo.Districts", "DistrictID");
        }
    }
}
