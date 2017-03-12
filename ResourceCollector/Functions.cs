using System;
using System.IO;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Pixel.Common.Cloud;
using System.Data.Entity;
using PixelApp.Models;
using Pixel.Common.Data;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ResourceCollector
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called addresources.
        public static void ProcessResourceMessage([QueueTrigger(QueueNames.ResourceQueue)] AddResourceMessage message, TextWriter log)
        {
            // tbd
        }

        public static void ProcessPopulationMessage([QueueTrigger(QueueNames.PopulationQueue)] AddPopulationMessage message)
        {
            // tbd
        }

        public static void ProcessNightlyAttack([QueueTrigger(QueueNames.NightlyAttackQueue)] NightlyAttackMessage message)
        {
            // tbd
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

        public static void ProcessZombieFight([QueueTrigger(QueueNames.ZombieFights)] ZombieFightMessage message)
        {
            // add to azure table storage
            var table = GetGameEventTable();

            var model = new ZombieFightEntity
            {
                DeltaLife = message.DeltaLife,
                DeltaXp = message.DeltaXp,
                IsDead = message.IsDead,
                IsWin = message.IsWin,
                UserId = message.UserId,
            };

            var op = TableOperation.Insert(model);

            table.Execute(op);

            //// check counts for badges
            //var query = new TableQuery<ZombieFightEntity>()
            //    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, TablePartitionKeys.GameEvents.ZombieFights))
            //    .Where(TableQuery.GenerateFilterCondition("UserId", QueryComparisons.Equal, message.UserId));

            //var fights = table.ExecuteQuery(query);

            //var wins = fights.Count(x => x.IsWin.Equals(true));
            //var losses = fights.Count(x => x.IsWin.Equals(false));
        }

        public static void ProcessFoodForage([QueueTrigger(QueueNames.FoodForage)] FoodForageMessage message)
        {
            // add to azure table storage
            var table = GetGameEventTable();

            var model = new FoodForageEntity
            {
                DeltaXp = message.DeltaXp,
                UserId = message.UserId,
                FoodFound = message.FoodFound,
            };

            var op = TableOperation.Insert(model);

            table.Execute(op);

            //// check counts for badges
            //var query = new TableQuery<FoodForageEntity>()
            //    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, TablePartitionKeys.GameEvents.FoodForages))
            //    .Where(TableQuery.GenerateFilterCondition("UserId", QueryComparisons.Equal, message.UserId));

            //var forageCount = table.ExecuteQuery(query).Count();


        }

        private static CloudTable GetGameEventTable()
        {
            var storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager
                            .ConnectionStrings["StorageConnectionString"].ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableNames.GameEvents);
            table.CreateIfNotExists();

            return table;
        }
    }
}
