namespace PixelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eventtimestamps : DbMigration
    {
        public override void Up()
        {
            var now = DateTime.Now;

            var previousMidnight = new DateTime(now.Year, now.Month, now.Day);
            AddColumn("dbo.Territories", "LastNightlyAttack", c => c.DateTime(nullable: false, defaultValue: previousMidnight));
            Sql($"update territories set lastnightlyattack='{now.Year}-{now.Month}-{now.Day}' where 1=1");

            var previousHour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            var previousHourText = $"{previousHour.Year}-{previousHour.Month}-{previousHour.Day} {previousHour.Hour}";
            AddColumn("dbo.Territories", "LastResourceCollection", c => c.DateTime(nullable: false, defaultValue: previousHour));
            Sql($"update territories set lastresourcecollection='{previousHourText}:00:00' where 1=1");

            // set the default value before making non-nullable
            Sql($"update territories set lastpopulationupdate='{now.Year}-{now.Month}-{now.Day}'");
            AlterColumn("dbo.Territories", "LastPopulationUpdate", c => c.DateTime(nullable: false));

            DropColumn("dbo.Territories", "LastResourceCollectionDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Territories", "LastResourceCollectionDate", c => c.DateTime());
            AlterColumn("dbo.Territories", "LastPopulationUpdate", c => c.DateTime());
            DropColumn("dbo.Territories", "LastResourceCollection");
            DropColumn("dbo.Territories", "LastNightlyAttack");
        }
    }
}
