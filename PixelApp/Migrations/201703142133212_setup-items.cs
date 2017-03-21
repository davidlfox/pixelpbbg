namespace PixelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class setupitems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserItems",
                c => new
                    {
                        UserItemId = c.Int(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        ItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserItemId)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ItemId);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        ItemId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        IsCore = c.Boolean(nullable: false),
                        MaxBoost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BoostType = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.ItemId);
            
            CreateTable(
                "dbo.ItemIngredients",
                c => new
                    {
                        ItemIngredientId = c.Int(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        ItemId = c.Int(nullable: false),
                        IngredientItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ItemIngredientId)
                .ForeignKey("dbo.Items", t => t.IngredientItemId)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .Index(t => t.ItemId)
                .Index(t => t.IngredientItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserItems", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserItems", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ItemIngredients", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ItemIngredients", "IngredientItemId", "dbo.Items");
            DropIndex("dbo.ItemIngredients", new[] { "IngredientItemId" });
            DropIndex("dbo.ItemIngredients", new[] { "ItemId" });
            DropIndex("dbo.UserItems", new[] { "ItemId" });
            DropIndex("dbo.UserItems", new[] { "UserId" });
            DropTable("dbo.ItemIngredients");
            DropTable("dbo.Items");
            DropTable("dbo.UserItems");
        }
    }
}
