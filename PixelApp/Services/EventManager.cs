using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

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

                    if (territory != null)
                    {
                        // add resources based on probability, allocation and intervals elapsed
                        // todo: boosts
                        user.Water += (int)(territory.WaterAllocation * territory.CivilianPopulation * hoursElapsed);
                        user.Wood += (int)(territory.WoodAllocation * territory.CivilianPopulation * hoursElapsed);
                        user.Food += (int)(territory.FoodAllocation * territory.CivilianPopulation * hoursElapsed);
                        user.Stone += (int)(territory.StoneAllocation * territory.CivilianPopulation * hoursElapsed);
                        user.Oil += (int)(territory.OilAllocation * territory.CivilianPopulation * hoursElapsed);
                        user.Iron += (int)(territory.IronAllocation * territory.CivilianPopulation * hoursElapsed);

                        // reset update time to the most recent hour to account for partial intervals
                        territory.LastResourceCollection = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
                    }
                }

                // check territory population growth
                if (territory.LastPopulationUpdate < DateTime.Now.AddHours(-24))
                {
                    var elapsed = DateTime.Now - territory.LastPopulationUpdate;
                    var daysElapsed = elapsed.Days;

                    var noteText = string.Empty;

                    // todo: boosts
                    var growth = (int)(territory.CivilianPopulation * territory.PopulationGrowthRate * daysElapsed);

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

                    for (var i = 0; i < daysElapsed; i++)
                    {
                        // 1/3 chance of nightly raid
                        if (rand.Next(0, 3) == 0)
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