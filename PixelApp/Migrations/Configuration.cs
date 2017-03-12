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
            SeedBadges(context);
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
                Permissions.CanEditBadges,
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
                var now = DateTime.Now;
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
                manager.AddToRole(user.Id, Permissions.CanEditBadges);

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
                    manager.AddToRole(adminUser.Id, Permissions.CanEditBadges);
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
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
                    ResearchDays = 1,
                    EnergyCost = 20,
                }
            );
        }

        private void SeedBadges(ApplicationDbContext context)
        {
            context.Badges.AddOrUpdate(x => x.BadgeId,
                new Badge
                {
                    BadgeId = 1,
                    BadgeType = BadgeTypes.ZombieKills,
                    Name = "Zombie Kills",
                    Description = "Kill 10 zombies while hunting!",
                    ExperienceGain = 50,
                    Level = 10,
                },
                new Badge
                {
                    BadgeId = 2,
                    BadgeType = BadgeTypes.ZombieKills,
                    Name = "Zombie Kills",
                    Description = "Kill 50 zombies while hunting!",
                    ExperienceGain = 200,
                    Level = 50,
                },
                new Badge
                {
                    BadgeId = 3,
                    BadgeType = BadgeTypes.ZombieKills,
                    Name = "Zombie Kills",
                    Description = "Kill 300 zombies while hunting!",
                    ExperienceGain = 1000,
                    Level = 300,
                },
                new Badge
                {
                    BadgeId = 4,
                    BadgeType = BadgeTypes.ZombieKills,
                    Name = "Zombie Kills",
                    Description = "Kill 1,000 zombies while hunting!",
                    ExperienceGain = 2500,
                    Level = 1000,
                },
                new Badge
                {
                    BadgeId = 5,
                    BadgeType = BadgeTypes.FoodForages,
                    Name = "Foraging",
                    Description = "Successfully forage for food 10 times!",
                    ExperienceGain = 75,
                    Level = 10,
                },
                new Badge
                {
                    BadgeId = 6,
                    BadgeType = BadgeTypes.FoodForages,
                    Name = "Foraging",
                    Description = "Successfully forage for food 50 times!",
                    ExperienceGain = 275,
                    Level = 50,
                },
                new Badge
                {
                    BadgeId = 7,
                    BadgeType = BadgeTypes.WaterCount,
                    Name = "Water Storage",
                    Description = "Successfully store 100 Water!",
                    ExperienceGain = 50,
                    Level = 100,
                },
                new Badge
                {
                    BadgeId = 8,
                    BadgeType = BadgeTypes.WaterCount,
                    Name = "Water Storage",
                    Description = "Successfully store 1,000 Water!",
                    ExperienceGain = 350,
                    Level = 1000,
                },
                new Badge
                {
                    BadgeId = 9,
                    BadgeType = BadgeTypes.WaterCount,
                    Name = "Water Storage",
                    Description = "Successfully store 10,000 Water!",
                    ExperienceGain = 2500,
                    Level = 10000,
                },
                new Badge
                {
                    BadgeId = 10,
                    BadgeType = BadgeTypes.WaterCount,
                    Name = "Water Storage",
                    Description = "Successfully store 100,000 Water!",
                    ExperienceGain = 50,
                    Level = 100000,
                },
                new Badge
                {
                    BadgeId = 11,
                    BadgeType = BadgeTypes.WoodCount,
                    Name = "Water Storage",
                    Description = "Successfully store 100 Wood!",
                    ExperienceGain = 50,
                    Level = 100,
                },
                new Badge
                {
                    BadgeId = 12,
                    BadgeType = BadgeTypes.WoodCount,
                    Name = "Water Storage",
                    Description = "Successfully store 1,000 Wood!",
                    ExperienceGain = 350,
                    Level = 1000,
                },
                new Badge
                {
                    BadgeId = 13,
                    BadgeType = BadgeTypes.WoodCount,
                    Name = "Water Storage",
                    Description = "Successfully store 10,000 Wood!",
                    ExperienceGain = 2500,
                    Level = 10000,
                },
                new Badge
                {
                    BadgeId = 14,
                    BadgeType = BadgeTypes.WoodCount,
                    Name = "Water Storage",
                    Description = "Successfully store 100,000 Wood!",
                    ExperienceGain = 50,
                    Level = 100000,
                },
                new Badge
                {
                    BadgeId = 15,
                    BadgeType = BadgeTypes.FoodCount,
                    Name = "Water Storage",
                    Description = "Successfully store 100 Food!",
                    ExperienceGain = 50,
                    Level = 100,
                },
                new Badge
                {
                    BadgeId = 16,
                    BadgeType = BadgeTypes.FoodCount,
                    Name = "Water Storage",
                    Description = "Successfully store 1,000 Food!",
                    ExperienceGain = 350,
                    Level = 1000,
                },
                new Badge
                {
                    BadgeId = 17,
                    BadgeType = BadgeTypes.FoodCount,
                    Name = "Water Storage",
                    Description = "Successfully store 10,000 Food!",
                    ExperienceGain = 2500,
                    Level = 10000,
                },
                new Badge
                {
                    BadgeId = 18,
                    BadgeType = BadgeTypes.FoodCount,
                    Name = "Water Storage",
                    Description = "Successfully store 100,000 Food!",
                    ExperienceGain = 50,
                    Level = 100000,
                },
                new Badge
                {
                    BadgeId = 19,
                    BadgeType = BadgeTypes.StoneCount,
                    Name = "Water Storage",
                    Description = "Successfully store 100 Stone!",
                    ExperienceGain = 50,
                    Level = 100,
                },
                new Badge
                {
                    BadgeId = 20,
                    BadgeType = BadgeTypes.StoneCount,
                    Name = "Water Storage",
                    Description = "Successfully store 1,000 Stone!",
                    ExperienceGain = 350,
                    Level = 1000,
                },
                new Badge
                {
                    BadgeId = 21,
                    BadgeType = BadgeTypes.StoneCount,
                    Name = "Water Storage",
                    Description = "Successfully store 10,000 Stone!",
                    ExperienceGain = 2500,
                    Level = 10000,
                },
                new Badge
                {
                    BadgeId = 22,
                    BadgeType = BadgeTypes.StoneCount,
                    Name = "Water Storage",
                    Description = "Successfully store 100,000 Stone!",
                    ExperienceGain = 50,
                    Level = 100000,
                },
                new Badge
                {
                    BadgeId = 23,
                    BadgeType = BadgeTypes.OilCount,
                    Name = "Water Storage",
                    Description = "Successfully store 100 Oil!",
                    ExperienceGain = 50,
                    Level = 100,
                },
                new Badge
                {
                    BadgeId = 24,
                    BadgeType = BadgeTypes.OilCount,
                    Name = "Water Storage",
                    Description = "Successfully store 1,000 Oil!",
                    ExperienceGain = 350,
                    Level = 1000,
                },
                new Badge
                {
                    BadgeId = 25,
                    BadgeType = BadgeTypes.OilCount,
                    Name = "Water Storage",
                    Description = "Successfully store 10,000 Oil!",
                    ExperienceGain = 2500,
                    Level = 10000,
                },
                new Badge
                {
                    BadgeId = 26,
                    BadgeType = BadgeTypes.OilCount,
                    Name = "Water Storage",
                    Description = "Successfully store 100,000 Oil!",
                    ExperienceGain = 50,
                    Level = 100000,
                },
                new Badge
                {
                    BadgeId = 27,
                    BadgeType = BadgeTypes.IronCount,
                    Name = "Water Storage",
                    Description = "Successfully store 100 Iron!",
                    ExperienceGain = 50,
                    Level = 100,
                },
                new Badge
                {
                    BadgeId = 28,
                    BadgeType = BadgeTypes.IronCount,
                    Name = "Water Storage",
                    Description = "Successfully store 1,000 Iron!",
                    ExperienceGain = 350,
                    Level = 1000,
                },
                new Badge
                {
                    BadgeId = 29,
                    BadgeType = BadgeTypes.IronCount,
                    Name = "Water Storage",
                    Description = "Successfully store 10,000 Iron!",
                    ExperienceGain = 2500,
                    Level = 10000,
                },
                new Badge
                {
                    BadgeId = 30,
                    BadgeType = BadgeTypes.IronCount,
                    Name = "Water Storage",
                    Description = "Successfully store 100,000 Iron!",
                    ExperienceGain = 50,
                    Level = 100000,
                },
                new Badge
                {
                    BadgeId = 31,
                    BadgeType = BadgeTypes.ResearchedSubjects,
                    Name = "Scholar",
                    Description = "Successfully researched 3 subjects!",
                    ExperienceGain = 300,
                    Level = 3,
                },
                new Badge
                {
                    BadgeId = 32,
                    BadgeType = BadgeTypes.ResearchedSubjects,
                    Name = "Scholar",
                    Description = "Successfully researched 9 subjects!",
                    ExperienceGain = 850,
                    Level = 9,
                },
                new Badge
                {
                    BadgeId = 33,
                    BadgeType = BadgeTypes.ResearchedSubjects,
                    Name = "Scholar",
                    Description = "Successfully researched 20 subjects!",
                    ExperienceGain = 1800,
                    Level = 20,
                },
                new Badge
                {
                    BadgeId = 34,
                    BadgeType = BadgeTypes.TerritoryNamed,
                    Name = "Settler",
                    Description = "Successfully name your territory!",
                    ExperienceGain = 40,
                    Level = 1,
                });
        }

    }
}
