namespace PixelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class badgeimage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Badges", "ImageUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Badges", "ImageUrl");
        }
    }
}
