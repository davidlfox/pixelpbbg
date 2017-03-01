namespace PixelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lastlogindate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "LastLoginDate", c => c.DateTime());

            Sql("update aspnetusers set lastlogindate=getdate() where 1=1;");
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "LastLoginDate");
        }
    }
}
