namespace PixelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Technology : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Technologies",
                c => new
                    {
                        TechnologyId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        TechnologyTypeId = c.Byte(nullable: false),
                        ResourceCostTypeId = c.Int(nullable: false),
                        ResourceCost = c.Int(nullable: false),
                        BoostTypeId = c.Byte(nullable: false),
                        BoostAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.TechnologyId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Technologies");
        }
    }
}
