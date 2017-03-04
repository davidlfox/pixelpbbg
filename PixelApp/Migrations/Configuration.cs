namespace PixelApp.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Pixel.Common.Data;
    using System.Collections.Generic;
    using Pixel.Common;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity;

    internal sealed class Configuration : DbMigrationsConfiguration<PixelApp.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = true;
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

            SeedPermissions(context);
            SeedUsers(context);
            TryToSetupAdmin(context);
            SeedTechnologies(context);
        }

        private static void SeedPermissions(ApplicationDbContext context)
        {
            var roles = new List<string>
            {
                Permissions.CanEditPermissions,
                Permissions.CanEditUsers,
                Permissions.CanEditAttackLogs,
                Permissions.CanEditTechnologies,
                Permissions.CanEditTerritories,
                Permissions.CanEditMOTD,
                Permissions.CanEditNotes,
                Permissions.CanTestController,
            };

            foreach (var roleName in roles)
            {
                if (!context.Roles.Any(r => r.Name == roleName))
                {
                    var store = new RoleStore<IdentityRole>(context);
                    var manager = new RoleManager<IdentityRole>(store);
                    var role = new IdentityRole { Name = roleName };

                    manager.Create(role);
                }
            }
        }

        private void SeedUsers(ApplicationDbContext context)
        {
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);

            // seed an admin
            var adminUserEmail = System.Configuration.ConfigurationManager.AppSettings["AdminUserEmail"];
            if (System.Configuration.ConfigurationManager.AppSettings["Environment"] == "dev" 
                && !context.Users.Any(x => x.Email == adminUserEmail))
            {
                var user = new ApplicationUser
                {
                    Email = adminUserEmail,
                    EmailConfirmed = true,
                    Energy = 100,
                    MaxEnergy = 100,
                    Life = 100,
                    MaxLife = 100,
                    Level = 1,
                    UserName = adminUserEmail,
                    Water = 50,
                    Wood = 50,
                    Food = 50,
                    Stone = 50,
                    Oil = 50,
                    Iron = 50,
                };

                var result = manager.Create(user, "123456");
                // give all roles
                manager.AddToRole(user.Id, Permissions.CanEditPermissions);
                manager.AddToRole(user.Id, Permissions.CanEditUsers);
                manager.AddToRole(user.Id, Permissions.CanEditAttackLogs);
                manager.AddToRole(user.Id, Permissions.CanEditTechnologies);
                manager.AddToRole(user.Id, Permissions.CanEditTerritories);
                manager.AddToRole(user.Id, Permissions.CanEditMOTD);
                manager.AddToRole(user.Id, Permissions.CanEditNotes);
                manager.AddToRole(user.Id, Permissions.CanTestController);

                var territory = new Territory();
                Services.TerritoryFactory.InitializeTerritory(territory);
                territory.Name = "me@a.com's territory";
                user.Territory = territory;
            }
        }

        private void TryToSetupAdmin(ApplicationDbContext context)
        {
            if(System.Configuration.ConfigurationManager.AppSettings["Environment"] == "prod")
            {
                var adminUserEmail = System.Configuration.ConfigurationManager.AppSettings["AdminUserEmail"];
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);

                // seed an admin
                var adminUser = context.Users.FirstOrDefault(x => x.Email == adminUserEmail);
                if (adminUser != null)
                {
                    manager.AddToRole(adminUser.Id, Permissions.CanEditPermissions);
                    manager.AddToRole(adminUser.Id, Permissions.CanEditUsers);
                    manager.AddToRole(adminUser.Id, Permissions.CanEditAttackLogs);
                    manager.AddToRole(adminUser.Id, Permissions.CanEditTechnologies);
                    manager.AddToRole(adminUser.Id, Permissions.CanEditTerritories);
                    manager.AddToRole(adminUser.Id, Permissions.CanEditMOTD);
                    manager.AddToRole(adminUser.Id, Permissions.CanEditNotes);
                    manager.AddToRole(adminUser.Id, Permissions.CanTestController);
                }
            }
        }

        private static void SeedTechnologies(ApplicationDbContext context)
        {
            // Add Technologies
            context.Technologies.AddOrUpdate(x => x.Name,
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
