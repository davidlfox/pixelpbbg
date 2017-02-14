namespace PixelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class scaffoldterritories : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Territories",
                c => new
                    {
                        TerritoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Type = c.Byte(nullable: false),
                        X = c.Int(nullable: false),
                        Y = c.Int(nullable: false),
                        Water = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Wood = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Coal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Stone = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Oil = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Iron = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CivilianPopulation = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TerritoryId);
            
            AddColumn("dbo.AspNetUsers", "TerritoryId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "TerritoryId");
            AddForeignKey("dbo.AspNetUsers", "TerritoryId", "dbo.Territories", "TerritoryId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "TerritoryId", "dbo.Territories");
            DropIndex("dbo.AspNetUsers", new[] { "TerritoryId" });
            DropColumn("dbo.AspNetUsers", "TerritoryId");
            DropTable("dbo.Territories");
        }
    }
}
