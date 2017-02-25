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
    }
}
