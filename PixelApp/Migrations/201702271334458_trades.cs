namespace PixelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class trades : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Trades",
                c => new
                    {
                        TradeId = c.Int(nullable: false, identity: true),
                        QuantityOffered = c.Int(nullable: false),
                        TypeOffered = c.Int(nullable: false),
                        QuantityAsked = c.Int(nullable: false),
                        TypeAsked = c.Int(nullable: false),
                        Posted = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        OwnerId = c.String(maxLength: 128),
                        TradedToUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.TradeId)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId)
                .ForeignKey("dbo.AspNetUsers", t => t.TradedToUserId)
                .Index(t => t.OwnerId)
                .Index(t => t.TradedToUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Trades", "TradedToUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Trades", "OwnerId", "dbo.AspNetUsers");
            DropIndex("dbo.Trades", new[] { "TradedToUserId" });
            DropIndex("dbo.Trades", new[] { "OwnerId" });
            DropTable("dbo.Trades");
        }
    }
}
