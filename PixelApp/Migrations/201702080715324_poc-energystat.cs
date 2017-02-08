namespace PixelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pocenergystat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Energy", c => c.Int(nullable: false, defaultValue: 100));
            AddColumn("dbo.AspNetUsers", "MaxEnergy", c => c.Int(nullable: false, defaultValue: 100));
            AddColumn("dbo.AspNetUsers", "EnergyUpdatedTime", c => c.DateTime());

            Sql("update aspnetusers set energy=100, maxenergy=100 where 1=1;");
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "EnergyUpdatedTime");
            DropColumn("dbo.AspNetUsers", "MaxEnergy");
            DropColumn("dbo.AspNetUsers", "Energy");
        }
    }
}
