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
                    var boosts = db.UserTechnologies.Where(x => x.UserId.Equals(user.Id)
                        && x.StatusId == UserTechnologyStatusTypes.Researched)
                        .Select(x => new
                        {
                            BoostTypeId = x.Technology.BoostTypeId,
                            BoostAmount = x.Technology.BoostAmount,
                        })
                        .ToList();

                    var civPop = territory.CivilianPopulation;


                    // add resources based on probability, allocation, intervals elapsed, and boosts
                    var boost = boosts.Where(x => x.BoostTypeId.Equals(BoostTypes.Water)).Sum(x => x.BoostAmount);
                    user.Water += (int)((territory.WaterAllocation + boost) * civPop * hoursElapsed);

                    boost = boosts.Where(x => x.BoostTypeId.Equals(BoostTypes.Wood)).Sum(x => x.BoostAmount);
                    user.Wood += (int)(territory.WoodAllocation * civPop * hoursElapsed);

                    boost = boosts.Where(x => x.BoostTypeId.Equals(BoostTypes.Food)).Sum(x => x.BoostAmount);
                    user.Food += (int)(territory.FoodAllocation * civPop * hoursElapsed);

                    boost = boosts.Where(x => x.BoostTypeId.Equals(BoostTypes.Stone)).Sum(x => x.BoostAmount);
                    user.Stone += (int)(territory.StoneAllocation * civPop * hoursElapsed);

                    boost = boosts.Where(x => x.BoostTypeId.Equals(BoostTypes.Oil)).Sum(x => x.BoostAmount);
                    user.Oil += (int)(territory.OilAllocation * civPop * hoursElapsed);

                    boost = boosts.Where(x => x.BoostTypeId.Equals(BoostTypes.Iron)).Sum(x => x.BoostAmount);
                    user.Iron += (int)(territory.IronAllocation * civPop * hoursElapsed);

                    // reset update time to the most recent hour to account for partial intervals
                    territory.LastResourceCollection = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);

                    // queue a message to check for badges
                    var qm = new QueueManager();
                    qm.QueueResourceCollection(user.Id, user.Water, user.Wood, user.Food, user.Stone, user.Oil, user.Iron);
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
    }
}