namespace PixelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class badges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Badges",
                c => new
                    {
                        BadgeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        BadgeType = c.Byte(nullable: false),
                        ExperienceGain = c.Int(nullable: false),
                        Level = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BadgeId);
            
            CreateTable(
                "dbo.UserBadges",
                c => new
                    {
                        UserBadgeId = c.Int(nullable: false, identity: true),
                        Created = c.DateTime(nullable: false),
                        UserId = c.String(maxLength: 128),
                        BadgeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserBadgeId)
                .ForeignKey("dbo.Badges", t => t.BadgeId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.BadgeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserBadges", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserBadges", "BadgeId", "dbo.Badges");
            DropIndex("dbo.UserBadges", new[] { "BadgeId" });
            DropIndex("dbo.UserBadges", new[] { "UserId" });
            DropTable("dbo.UserBadges");
            DropTable("dbo.Badges");
        }
    }
}
