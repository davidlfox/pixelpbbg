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
                        ResourceTypeId = c.Int(nullable: false),
                        ResourceCost = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TechnologyId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Technologies");
        }
    }
}
