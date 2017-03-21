using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Pixel.Common.Data;
using Pixel.Common.Cloud;

namespace PixelApp.Services
{
    public class EventManager
    {
        public void ProcessEvents(ApplicationDbContext db, ApplicationUser user)
        {
            var territory = db.Territories
                .Include(x => x.Players)
                .FirstOrDefault(x => x.TerritoryId == user.TerritoryId);

            var now = DateTime.Now;

            if (territory != null)
            {
                // check resource updates
                if (territory.LastResourceCollection < DateTime.Now.AddHours(-1))
                {
                    // determine how many intervals
                    var elapsed = DateTime.Now - territory.LastResourceCollection;
                    var hoursElapsed = elapsed.Hours;

                    // Get boosts from technology
                    var techBoosts = db.UserTechnologies
                        .Include(x => x.Technology)
                        .Where(x => x.UserId.Equals(user.Id) && x.StatusId == UserTechnologyStatusTypes.Researched)
                        .ToList();

                    var civPop = territory.CivilianPopulation;

                    // todo: this sucks, refactor allocations into a table or something
                    var resourceMeta = db.Items.Where(x => x.IsCore.Equals(true))
                        .Select(x => new ResourceMeta
                        {
                            ItemId = x.ItemId,
                            // this will be filled out momentarily
                            NewQuantity = 0,
                        })
                        .ToList();

                    var userItems = user.Items
                        .OrderByDescending(x => x.Item.MaxBoost)
                        .ToList();

                    foreach (var resource in db.Items.Where(x => x.IsCore.Equals(true)))
                    {
                        var allocation = 0.0m;
                        // todo: this sucks, refactor allocations into a table or something
                        switch (resource.ItemId)
                        {
                            case 1: allocation = territory.WaterAllocation; break;
                            case 2: allocation = territory.FoodAllocation; break;
                            case 3: allocation = territory.WoodAllocation; break;
                            case 4: allocation = territory.StoneAllocation; break;
                            case 5: allocation = territory.OilAllocation; break;
                            case 6: allocation = territory.IronAllocation; break;
                        }

                        // get the useritem record
                        var resourceItem = user.Items.Single(x => x.ItemId == resource.ItemId);
                        // start with the boost from technologies
                        var boost = techBoosts
                            .Where(x => x.Technology.BoostTypeId.Equals((BoostTypes)resource.ItemId))
                            .Sum(x => x.Technology.BoostAmount);

                        // add any boost from items
                        var boostItems = userItems
                            .Where(x => x.Item.BoostType == (BoostTypes)resource.ItemId)
                            .Select(x => new BoostItem
                            {
                                BoostType = x.Item.BoostType,
                                MaxBoost = x.Item.MaxBoost,
                                Quantity = x.Quantity,
                            });

                        if (boostItems.Any())
                        {
                            boost += GetItemBoost((BoostTypes)resource.ItemId, boostItems.ToList(), allocation, civPop);
                        }
                        // do resource collection
                        resourceItem.Quantity += (int)((allocation + boost) * civPop * hoursElapsed);
                        resourceMeta.Single(x => x.ItemId == resource.ItemId).NewQuantity = resourceItem.Quantity;
                    }

                    // reset update time to the most recent hour to account for partial intervals
                    territory.LastResourceCollection = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);

                    // queue a message to check for badges
                    var qm = new QueueManager();
                    qm.QueueResourceCollection(user.Id,
                        resourceMeta.Single(x => x.ItemId == (int)ResourceTypes.Water).NewQuantity,
                        resourceMeta.Single(x => x.ItemId == (int)ResourceTypes.Food).NewQuantity,
                        resourceMeta.Single(x => x.ItemId == (int)ResourceTypes.Wood).NewQuantity,
                        resourceMeta.Single(x => x.ItemId == (int)ResourceTypes.Stone).NewQuantity,
                        resourceMeta.Single(x => x.ItemId == (int)ResourceTypes.Oil).NewQuantity,
                        resourceMeta.Single(x => x.ItemId == (int)ResourceTypes.Iron).NewQuantity);
                }

                // check territory population growth
                if (territory.LastPopulationUpdate < DateTime.Now.AddHours(-24))
                {
                    var elapsed = DateTime.Now - territory.LastPopulationUpdate;
                    var daysElapsed = elapsed.Days;

                    var noteText = string.Empty;

                    // Get boosts from technology
                    var populationBoosts = db.UserTechnologies.Where(x => x.UserId.Equals(user.Id)
                        && x.StatusId == UserTechnologyStatusTypes.Researched
                        && x.Technology.BoostTypeId == BoostTypes.Population)
                        .Select(x => x.Technology.BoostAmount).ToList();

                    var growth = (int)(territory.CivilianPopulation * (territory.PopulationGrowthRate + populationBoosts.Sum()) * daysElapsed);

                    territory.CivilianPopulation += growth;

                    // reset update time to account for partial intervals
                    territory.LastPopulationUpdate = new DateTime(now.Year, now.Month, now.Day);

                    // notify user
                    var timeText = daysElapsed > 1 ? "recently" : "last night";
                    noteText = $"A few people from the outskirts found their way into your territory {timeText}. " +
                                $"Your population grew by {growth} to {territory.CivilianPopulation}. " +
                                "Your increased population will automatically help you gather more resources.";

                    var note = CommunicationService.CreateNotification(
                        user.Id,
                        $"Your civilian population grew by {growth} last night!",
                        noteText);

                    db.Notes.Add(note);
                }

                // check nightly attacks
                if (territory.LastNightlyAttack < DateTime.Now.AddHours(-24))
                {
                    var elapsed = DateTime.Now - territory.LastNightlyAttack;
                    var daysElapsed = elapsed.Days;

                    var rand = new Random();

                    var log = new AttackLog
                    {
                        UserId = user.Id,
                    };

                    var survivals = 0;
                    var attacks = 0;
                    var populationLoss = 0;
                    var resourceLossText = string.Empty;

                    // Calculate winPercentage
                    var defenseBoosts = db.UserTechnologies.Where(x => x.UserId.Equals(user.Id)
                        && x.StatusId == UserTechnologyStatusTypes.Researched
                        && x.Technology.BoostTypeId == BoostTypes.Defense)
                        .Select(x => x.Technology.BoostAmount).ToList();
                    var winPercentage = 67 + (defenseBoosts.Sum() * 100);

                    for (var i = 0; i < daysElapsed; i++)
                    {
                        if (rand.Next(0, 100) > winPercentage)
                        {
                            attacks++;
                            log.WasAttacked = true;

                            // 1-2 nightly population loss for right now
                            // todo: boosts
                            populationLoss += rand.Next(0, 2) + 1;

                            // lose random resources
                            resourceLossText = ResourceService.RandomResource(user, false, false, 0.05);
                        }
                        else
                        {
                            survivals++;
                        }
                    }

                    // total population loss since last calculation
                    territory.CivilianPopulation -= populationLoss;

                    // reset update time to account for partial intervals
                    territory.LastNightlyAttack = new DateTime(now.Year, now.Month, now.Day);

                    if (attacks > 1)
                    {
                        log.Message = string.Format(
                            "Zombies have been pillaging your territory while you were away. You lost {0} {1}. {2}" +
                                $"Zombie Attacks: {attacks}. Thwarted attempts: {survivals}"
                            , populationLoss
                            , populationLoss == 1 ? "person" : "people"
                            , "You lost too many resources to count!");
                    }
                    else if (attacks == 1)
                    {
                        log.Message = string.Format(
                            "Zombies attacked your territory last night. You lost {0} {1}. {2}"
                            , populationLoss
                            , populationLoss == 1 ? "person" : "people"
                            , resourceLossText);
                    }
                    else if (attacks == 0)
                    {
                        log.Message = "Your territory survived all recent zombie attacks.";
                    }

                    var note = CommunicationService.CreateNotification(
                        user.Id,
                        "Recent Zombie Activity",
                        log.Message);

                    db.Notes.Add(note);

                    db.AttackLogs.Add(log);
                }
            }
        }

        private decimal GetItemBoost(BoostTypes boostType, List<BoostItem> boostItems, decimal allocation, int population)
        {
            var boost = 0.0m;
            // get highest item boosts first e.g. if 100 people allocated, get 50 1.5% items, then 50 1% items, and weight boost
            var peopleAllocated = allocation * population;
            var peopleWithItems = 0;
            var itemsLeft = boostItems.Count();
            var index = 0;

            // try to have each person use an item
            while (peopleWithItems < peopleAllocated && itemsLeft > 0 && boostItems.Count() > index)
            {
                var boostItem = boostItems.ElementAt(index++);
                var quantity = boostItem.Quantity;
                // this many people still need an item
                var canAllocateQty = peopleAllocated - peopleWithItems;
                // add the fewest items allowed
                var peopleToGiveItem = Math.Min(quantity, canAllocateQty);
                var percentOfMaxBoost = peopleToGiveItem / peopleAllocated;
                // calculate relative boost e.g. if they have enough items to fill half allocated pop. then they get half max boost
                // add to existing boost (so far, just technology above)
                boost += percentOfMaxBoost * boostItem.MaxBoost / 100;
            }

            return boost;
        }
    }

    public class ResourceMeta
    {
        public int ItemId { get; set; }
        public int NewQuantity { get; set; }
    }

    public class BoostItem
    {
        public int Quantity { get; set; }
        public BoostTypes BoostType { get; set; }
        public decimal MaxBoost { get; set; }
    }
}