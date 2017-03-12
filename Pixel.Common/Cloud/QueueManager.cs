using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixel.Common.Cloud
{
    public class QueueManager
    {
        private CloudQueueClient queueClient;

        public QueueManager()
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());
            this.queueClient = storageAccount.CreateCloudQueueClient();
        }

        [Obsolete("Resource collections are checked in real-time")]
        public void QueueResourceCollection(int territoryId, int delayInMinutes = 60)
        {
            var queue = this.queueClient.GetQueueReference(QueueNames.ResourceQueue);
            queue.CreateIfNotExists();
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(new AddResourceMessage
            {
                TerritoryId = territoryId,
            }));

            queue.AddMessageAsync(message, null, TimeSpan.FromMinutes(delayInMinutes), null, null);
        }

        [Obsolete("Population growth events are checked in real-time")]
        public void QueuePopulation(int territoryId, int delayInMinutes = 1440)
        {
            var queue = this.queueClient.GetQueueReference(QueueNames.PopulationQueue);
            queue.CreateIfNotExists();
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(new AddPopulationMessage
            {
                TerritoryId = territoryId,
            }));

            queue.AddMessageAsync(message, null, TimeSpan.FromMinutes(delayInMinutes), null, null);
        }

        [Obsolete("Nightly attacks are checked in real-time")]
        public void QueueNightlyAttack(int territoryId, int delayInMinutes = 1440)
        {
            var queue = this.queueClient.GetQueueReference(QueueNames.NightlyAttackQueue);
            queue.CreateIfNotExists();
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(new NightlyAttackMessage
            {
                TerritoryId = territoryId,
            }));

            queue.AddMessageAsync(message, null, TimeSpan.FromMinutes(delayInMinutes), null, null);
        }

        public void QueueExperience(string userId, int experience, int delayInMinutes = 0)
        {
            var queue = this.queueClient.GetQueueReference(QueueNames.Experience);
            queue.CreateIfNotExists();
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(new ExperienceMessage
            {
                UserId = userId,
                Experience = experience,
            }));

            queue.AddMessageAsync(message, null, TimeSpan.FromMinutes(delayInMinutes), null, null);
        }

        public async Task QueueZombieFight(string userId, int deltaXp, int deltaLife, bool isWin, bool isDead)
        {
            var queue = this.queueClient.GetQueueReference(QueueNames.ZombieFights);
            queue.CreateIfNotExists();
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(new ZombieFightMessage
            {
                UserId = userId,
                DeltaXp = deltaXp,
                DeltaLife = deltaLife,
                IsWin = isWin,
                IsDead = isDead,
            }));

            await queue.AddMessageAsync(message);
        }

        public async Task QueueFoodForage(string userId, int deltaXp, int foodFound)
        {
            var queue = this.queueClient.GetQueueReference(QueueNames.FoodForage);
            queue.CreateIfNotExists();
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(new FoodForageMessage
            {
                UserId = userId,
                DeltaXp = deltaXp,
                FoodFound = foodFound,
            }));

            await queue.AddMessageAsync(message);
        }
    }
}
