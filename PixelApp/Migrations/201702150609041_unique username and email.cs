namespace PixelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uniqueusernameandemail : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.AspNetUsers", "UserName", unique: true);
            CreateIndex("dbo.AspNetUsers", "Email", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.AspNetUsers", new[] { "Email" });
            DropIndex("dbo.AspNetUsers", new[] { "UserName" });
        }
    }
}
