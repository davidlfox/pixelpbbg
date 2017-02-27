using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Pixel.Common.Cloud;
using PixelApp.Models;
using System.Data.Entity;
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
                .Single(x => x.TerritoryId == message.TerritoryId);
            var userId = territory.Players.First().Id;
            var user = db.Users.Single(x => x.Id == userId);

            // add resources based on probability and allocation
            user.Water += (int)(territory.Water * territory.WaterAllocation * territory.CivilianPopulation);
            user.Wood += (int)(territory.Wood * territory.WoodAllocation * territory.CivilianPopulation);
            user.Food += (int)(territory.Food * territory.FoodAllocation * territory.CivilianPopulation);
            user.Stone += (int)(territory.Stone * territory.StoneAllocation * territory.CivilianPopulation);
            user.Oil += (int)(territory.Oil * territory.OilAllocation * territory.CivilianPopulation);
            user.Iron += (int)(territory.Iron * territory.IronAllocation * territory.CivilianPopulation);

            territory.LastResourceCollectionDate = DateTime.Now;

            db.SaveChanges();

            // queue next message
            var qm = new QueueManager();
            qm.QueueResourceCollection(territory.TerritoryId);
        }

        public static void ProcessPopulationMessage([QueueTrigger(QueueNames.PopulationQueue)] AddPopulationMessage message)
        {
            var db = new ApplicationDbContext();
            var territory = db.Territories.Single(x => x.TerritoryId == message.TerritoryId);

            if(message.Population > 0)
            {
                territory.CivilianPopulation += message.Population;
            }
            else
            {
                territory.CivilianPopulation += (int)(territory.CivilianPopulation * territory.PopulationGrowthRate);
            }

            territory.LastPopulationUpdate = DateTime.Now;

            db.SaveChanges();

            // queue next message
            var qm = new QueueManager();
            qm.QueuePopulation(territory.TerritoryId);
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
                .Single(x => x.TerritoryId == message.TerritoryId);

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

            db.AttackLogs.Add(log);
            db.SaveChanges();

            // queue next message
            var qm = new QueueManager();
            qm.QueueNightlyAttack(territory.TerritoryId);
        }
    }
}
