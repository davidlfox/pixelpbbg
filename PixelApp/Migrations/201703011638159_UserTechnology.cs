namespace PixelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserTechnology : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserTechnologies",
                c => new
                    {
                        UserTechnologyId = c.Int(nullable: false, identity: true),
                        TechnologyId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        ResearchStartDate = c.DateTime(nullable: false),
                        ResearchDays = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserTechnologyId)
                .ForeignKey("dbo.Technologies", t => t.TechnologyId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.TechnologyId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTechnologies", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserTechnologies", "TechnologyId", "dbo.Technologies");
            DropIndex("dbo.UserTechnologies", new[] { "UserId" });
            DropIndex("dbo.UserTechnologies", new[] { "TechnologyId" });
            DropTable("dbo.UserTechnologies");
        }
    }
}
