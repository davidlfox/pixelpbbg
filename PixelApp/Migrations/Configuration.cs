namespace PixelApp.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Pixel.Common.Data;

    internal sealed class Configuration : DbMigrationsConfiguration<PixelApp.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PixelApp.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            // Add Technologies
            context.Technologies.AddOrUpdate(x => x.TechnologyId,
                new Models.Technology
                {
                    Name = "Wind Powered Pump",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Resource,
                    ResourceCostTypeId = ResourceTypes.Wood,
                    ResourceCost = 100,
                    BoostTypeId = BoostTypes.Water,
                    BoostAmount = .02m,
                },
                new Models.Technology
                {
                    Name = "ChainSaw",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Resource,
                    ResourceCostTypeId = ResourceTypes.Iron,
                    ResourceCost = 100,
                    BoostTypeId = BoostTypes.Wood,
                    BoostAmount = .02m,
                },
                new Models.Technology
                {
                    Name = "Harvester",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Resource,
                    ResourceCostTypeId = ResourceTypes.Iron,
                    ResourceCost = 100,
                    BoostTypeId = BoostTypes.Food,
                    BoostAmount = .02m,
                },
                new Models.Technology
                {
                    Name = "Quarry Improvements",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Resource,
                    ResourceCostTypeId = ResourceTypes.Wood,
                    ResourceCost = 100,
                    BoostTypeId = BoostTypes.Stone,
                    BoostAmount = .02m,
                },
                new Models.Technology
                {
                    Name = "Simple Oil Derrick",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Resource,
                    ResourceCostTypeId = ResourceTypes.Wood,
                    ResourceCost = 100,
                    BoostTypeId = BoostTypes.Oil,
                    BoostAmount = .02m,
                },
                new Models.Technology
                {
                    Name = "Blast Furnace",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Resource,
                    ResourceCostTypeId = ResourceTypes.Oil,
                    ResourceCost = 100,
                    BoostTypeId = BoostTypes.Iron,
                    BoostAmount = .02m,
                },
                new Models.Technology
                {
                    Name = "Bolt Action Rifles",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Military,
                    ResourceCostTypeId = ResourceTypes.Iron,
                    ResourceCost = 100,
                    BoostTypeId = BoostTypes.Offense,
                    BoostAmount = .02m,
                },
                new Models.Technology
                {
                    Name = "Revolvers",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Military,
                    ResourceCostTypeId = ResourceTypes.Wood,
                    ResourceCost = 100,
                    BoostTypeId = BoostTypes.Offense,
                    BoostAmount = .02m,
                },
                new Models.Technology
                {
                    Name = "Simple Explosives",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Military,
                    ResourceCostTypeId = ResourceTypes.Oil,
                    ResourceCost = 100,
                    BoostTypeId = BoostTypes.Offense,
                    BoostAmount = .02m,
                },
                new Models.Technology
                {
                    Name = "Body Armor",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Military,
                    ResourceCostTypeId = ResourceTypes.Iron,
                    ResourceCost = 100,
                    BoostTypeId = BoostTypes.Defense,
                    BoostAmount = .02m,
                },
                new Models.Technology
                {
                    Name = "Camouflage",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Military,
                    ResourceCostTypeId = ResourceTypes.Wood,
                    ResourceCost = 100,
                    BoostTypeId = BoostTypes.Defense,
                    BoostAmount = .02m,
                },
                new Models.Technology
                {
                    Name = "Short Wave Radio",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Military,
                    ResourceCostTypeId = ResourceTypes.Iron,
                    ResourceCost = 100,
                    BoostTypeId = BoostTypes.Defense,
                    BoostAmount = .02m,
                },
                new Models.Technology
                {
                    Name = "Wooden Fortifications",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Culture,
                    ResourceCostTypeId = ResourceTypes.Wood,
                    ResourceCost = 100,
                    BoostTypeId = BoostTypes.Population,
                    BoostAmount = .02m,
                },
                new Models.Technology
                {
                    Name = "WatchTowers",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Culture,
                    ResourceCostTypeId = ResourceTypes.Wood,
                    ResourceCost = 100,
                    BoostTypeId = BoostTypes.Population,
                    BoostAmount = .02m,
                },
                new Models.Technology
                {
                    Name = "Police Force",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Culture,
                    ResourceCostTypeId = ResourceTypes.Oil,
                    ResourceCost = 100,
                    BoostTypeId = BoostTypes.Population,
                    BoostAmount = .02m,
                },
                new Models.Technology
                {
                    Name = "Community Centers",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Culture,
                    ResourceCostTypeId = ResourceTypes.Food,
                    ResourceCost = 100,
                    BoostTypeId = BoostTypes.Population,
                    BoostAmount = .02m,
                },
                new Models.Technology
                {
                    Name = "Community Housing",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Culture,
                    ResourceCostTypeId = ResourceTypes.Stone,
                    ResourceCost = 100,
                    BoostTypeId = BoostTypes.Population,
                    BoostAmount = .02m,
                },
                new Models.Technology
                {
                    Name = "Infrastructure",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Culture,
                    ResourceCostTypeId = ResourceTypes.Water,
                    ResourceCost = 100,
                    BoostTypeId = BoostTypes.Population,
                    BoostAmount = .02m,
                },
                new Models.Technology
                {
                    Name = "Stone Fortifications",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Culture,
                    ResourceCostTypeId = ResourceTypes.Stone,
                    ResourceCost = 500,
                    BoostTypeId = BoostTypes.Population,
                    BoostAmount = .03m,
                },
                new Models.Technology
                {
                    Name = "Plumbing",
                    Description = "TODO",
                    TechnologyTypeId = TechnologyTypes.Culture,
                    ResourceCostTypeId = ResourceTypes.Water,
                    ResourceCost = 500,
                    BoostTypeId = BoostTypes.Population,
                    BoostAmount = .03m,
                }
            );
        }
    }
}
