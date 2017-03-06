using System;
using System.IO;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Pixel.Common.Cloud;
using System.Data.Entity;
using PixelApp.Models;
using PixelApp.Services;

namespace ResourceCollector
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called addresources.
        public static void ProcessResourceMessage([QueueTrigger(QueueNames.ResourceQueue)] AddResourceMessage message, TextWriter log)
        {
            var db = new ApplicationDbContext();
            var territory = db.Territories
                .Include(x => x.Players)
                .FirstOrDefault(x => x.TerritoryId == message.TerritoryId);

            if (territory != null)
            {
                var userId = territory.Players.First().Id;
                var user = db.Users.Single(x => x.Id == userId);

                // add resources based on probability and allocation
                user.Water += (int)(territory.WaterAllocation * territory.CivilianPopulation);
                user.Wood += (int)(territory.WoodAllocation * territory.CivilianPopulation);
                user.Food += (int)(territory.FoodAllocation * territory.CivilianPopulation);
                user.Stone += (int)(territory.StoneAllocation * territory.CivilianPopulation);
                user.Oil += (int)(territory.OilAllocation * territory.CivilianPopulation);
                user.Iron += (int)(territory.IronAllocation * territory.CivilianPopulation);

                territory.LastResourceCollectionDate = DateTime.Now;

                db.SaveChanges();

                // queue next message
                var qm = new QueueManager();
                qm.QueueResourceCollection(territory.TerritoryId);
            }
        }

        public static void ProcessPopulationMessage([QueueTrigger(QueueNames.PopulationQueue)] AddPopulationMessage message)
        {
            var db = new ApplicationDbContext();
            var territory = db.Territories
                .Include(x => x.Players)
                .FirstOrDefault(x => x.TerritoryId == message.TerritoryId);

            if (territory != null)
            {
                var noteText = string.Empty;
                var growth = message.Population;

                if(message.Population > 0)
                {
                    territory.CivilianPopulation += message.Population;
                }
                else
                {
                    growth = (int)(territory.CivilianPopulation * territory.PopulationGrowthRate);
                    territory.CivilianPopulation += growth;
                }

                territory.LastPopulationUpdate = DateTime.Now;

                noteText = "A few people from the outskirts found their way into your territory last night. " +
                           $"Your population grew by {growth} to {territory.CivilianPopulation}. " +
                           "Your increased population will automatically help you gather more resources.";

                var user = territory.Players.First();

                // notify user
                var note = CommunicationService.CreateNotification(
                    user.Id,
                    $"Your civilian population grew by {growth} last night!",
                    noteText);

                db.Notes.Add(note);

                db.SaveChanges();

                // queue next message
                var qm = new QueueManager();
                qm.QueuePopulation(territory.TerritoryId);
            }
        }

        /// <summary>
        /// Handle a nightly attack queue message
        /// </summary>
        /// <param name="message">The message signaling chance of an attack</param>
        public static void ProcessNightlyAttack([QueueTrigger(QueueNames.NightlyAttackQueue)] NightlyAttackMessage message)
        {
            var db = new ApplicationDbContext();
            var territory = db.Territories
                .Include(x => x.Players)
                .FirstOrDefault(x => x.TerritoryId == message.TerritoryId);

            if (territory != null)
            {
                var user = territory.Players.First();

                var rand = new Random();

                var log = new AttackLog
                {
                    UserId = user.Id,
                };

                // 1/3 chance of nightly raid
                if (rand.Next(0, 3) == 0)
                {
                    log.WasAttacked = true;

                    // 1-2 nightly population loss for right now
                    var populationLoss = rand.Next(0, 2) + 1;
                    territory.CivilianPopulation -= populationLoss;

                    // choose random resources the player has and remove them
                    var resourceLossText = ResourceService.RandomResource(user, false, false, 0.05);

                    log.Message = string.Format("Zombies attacked your territory last night. You lost {0} {1}. {2}"
                        , populationLoss
                        , populationLoss == 1 ? "person" : "people"
                        , resourceLossText);
                }
                else
                {
                    log.Message = "Your territory survived minor zombie attacks last night.";
                }

                var note = CommunicationService.CreateNotification(
                    user.Id,
                    "Zombies attacked last night!",
                    log.Message);

                db.Notes.Add(note);

                db.AttackLogs.Add(log);
                db.SaveChanges();

                // queue next message
                var qm = new QueueManager();
                qm.QueueNightlyAttack(territory.TerritoryId);
            }
        }

        public static void ProcessExperience([QueueTrigger(QueueNames.Experience)] ExperienceMessage message)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Single(x => x.Id == message.UserId);
                user.Experience += message.Experience;
                db.SaveChanges();
            }
        }
    }
}
