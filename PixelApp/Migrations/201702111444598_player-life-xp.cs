namespace PixelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class playerlifexp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Life", c => c.Int(nullable: false, defaultValue: 100));
            AddColumn("dbo.AspNetUsers", "MaxLife", c => c.Int(nullable: false, defaultValue: 100));
            AddColumn("dbo.AspNetUsers", "LifeUpdatedTime", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "Level", c => c.Byte(nullable: false, defaultValue: 1));
            AddColumn("dbo.AspNetUsers", "Experience", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Experience");
            DropColumn("dbo.AspNetUsers", "Level");
            DropColumn("dbo.AspNetUsers", "LifeUpdatedTime");
            DropColumn("dbo.AspNetUsers", "MaxLife");
            DropColumn("dbo.AspNetUsers", "Life");
        }
    }
}
