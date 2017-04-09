namespace PixelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paypal : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PaypalTransactions",
                c => new
                    {
                        PaypalTransactionId = c.Int(nullable: false, identity: true),
                        Product = c.String(),
                        TransactionId = c.String(),
                        Quantity = c.Int(nullable: false),
                        Fee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentStatus = c.String(),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.PaypalTransactionId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);

            AddColumn("dbo.AspNetUsers", "HourlyResourceBoosts", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PaypalTransactions", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.PaypalTransactions", new[] { "UserId" });
            DropColumn("dbo.AspNetUsers", "HourlyResourceBoosts");
            DropTable("dbo.PaypalTransactions");
        }
    }
}
