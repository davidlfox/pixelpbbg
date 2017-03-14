namespace PixelApp.Migrations
{
    using Models;
    using System;
    using System.Data.Entity.Migrations;

    public partial class copyresourcestoitems : DbMigration
    {
        public override void Up()
        {
            var db = new ApplicationDbContext();
            foreach (var user in db.Users)
            {
                db.UserItems.Add(new UserItem { ItemId = 1, Quantity = user.Water, UserId = user.Id });
                db.UserItems.Add(new UserItem { ItemId = 2, Quantity = user.Food, UserId = user.Id });
                db.UserItems.Add(new UserItem { ItemId = 3, Quantity = user.Wood, UserId = user.Id });
                db.UserItems.Add(new UserItem { ItemId = 4, Quantity = user.Stone, UserId = user.Id });
                db.UserItems.Add(new UserItem { ItemId = 5, Quantity = user.Oil, UserId = user.Id });
                db.UserItems.Add(new UserItem { ItemId = 6, Quantity = user.Iron, UserId = user.Id });
            }

            db.SaveChanges();
        }
        
        public override void Down()
        {
            
        }
    }
}
