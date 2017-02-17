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
    }
}
