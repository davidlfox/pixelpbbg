namespace PixelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class techprereq : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Technologies", "PreRequisiteId", c => c.Int());
            AddColumn("dbo.Technologies", "EnergyCost", c => c.Int(nullable: false));
            CreateIndex("dbo.Technologies", "PreRequisiteId");
            AddForeignKey("dbo.Technologies", "PreRequisiteId", "dbo.Technologies", "TechnologyId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Technologies", "PreRequisiteId", "dbo.Technologies");
            DropIndex("dbo.Technologies", new[] { "PreRequisiteId" });
            DropColumn("dbo.Technologies", "EnergyCost");
            DropColumn("dbo.Technologies", "PreRequisiteId");
        }
    }
}
