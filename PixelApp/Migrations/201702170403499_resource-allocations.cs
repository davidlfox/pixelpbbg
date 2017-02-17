namespace PixelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class resourceallocations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Territories", "PopulationGrowthRate", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0.02m));
            AddColumn("dbo.Territories", "LastPopulationUpdate", c => c.DateTime());
            AddColumn("dbo.Territories", "WaterAllocation", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0.17m));
            AddColumn("dbo.Territories", "WoodAllocation", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0.17m));
            AddColumn("dbo.Territories", "CoalAllocation", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0.17m));
            AddColumn("dbo.Territories", "StoneAllocation", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0.17m));
            AddColumn("dbo.Territories", "OilAllocation", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0.16m));
            AddColumn("dbo.Territories", "IronAllocation", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0.16m));
            AddColumn("dbo.Territories", "LastResourceCollectionDate", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "Wood", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Water", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Coal", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Stone", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Oil", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Iron", c => c.Int(nullable: false));

            Sql("update territories set civilianpopulation=120 where civilianpopulation=0;");
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Iron");
            DropColumn("dbo.AspNetUsers", "Oil");
            DropColumn("dbo.AspNetUsers", "Stone");
            DropColumn("dbo.AspNetUsers", "Coal");
            DropColumn("dbo.AspNetUsers", "Water");
            DropColumn("dbo.AspNetUsers", "Wood");
            DropColumn("dbo.Territories", "LastResourceCollectionDate");
            DropColumn("dbo.Territories", "IronAllocation");
            DropColumn("dbo.Territories", "OilAllocation");
            DropColumn("dbo.Territories", "StoneAllocation");
            DropColumn("dbo.Territories", "CoalAllocation");
            DropColumn("dbo.Territories", "WoodAllocation");
            DropColumn("dbo.Territories", "WaterAllocation");
            DropColumn("dbo.Territories", "LastPopulationUpdate");
            DropColumn("dbo.Territories", "PopulationGrowthRate");
        }
    }
}
