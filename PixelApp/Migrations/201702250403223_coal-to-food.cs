namespace PixelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class coaltofood : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Territories", "Food", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Territories", "FoodAllocation", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0.17m));
            AddColumn("dbo.AspNetUsers", "Food", c => c.Int(nullable: false));

            // transfer coal quantity to food
            Sql("update aspnetusers set food=coal where 1=1");

            DropColumn("dbo.Territories", "Coal");
            DropColumn("dbo.Territories", "CoalAllocation");
            DropColumn("dbo.AspNetUsers", "Coal");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Coal", c => c.Int(nullable: false));
            AddColumn("dbo.Territories", "CoalAllocation", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Territories", "Coal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.AspNetUsers", "Food");
            DropColumn("dbo.Territories", "FoodAllocation");
            DropColumn("dbo.Territories", "Food");
        }
    }
}
