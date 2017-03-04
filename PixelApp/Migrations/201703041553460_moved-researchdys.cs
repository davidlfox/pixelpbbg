namespace PixelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class movedresearchdys : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Technologies", "ResearchDays", c => c.Int(nullable: false));
            DropColumn("dbo.UserTechnologies", "ResearchDays");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserTechnologies", "ResearchDays", c => c.Int(nullable: false));
            DropColumn("dbo.Technologies", "ResearchDays");
        }
    }
}
