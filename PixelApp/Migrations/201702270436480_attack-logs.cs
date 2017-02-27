namespace PixelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class attacklogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AttackLogs",
                c => new
                    {
                        AttackLogId = c.Int(nullable: false, identity: true),
                        TimeOfAttack = c.DateTime(nullable: false),
                        WasAttacked = c.Boolean(nullable: false),
                        Message = c.String(),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.AttackLogId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AttackLogs", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AttackLogs", new[] { "UserId" });
            DropTable("dbo.AttackLogs");
        }
    }
}
