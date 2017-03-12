using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Pixel.Common;
using Pixel.Common.Cloud;
using Pixel.Common.Data;
using PixelApp.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Controllers
{
    [Authorize(Roles = Permissions.CanTestController)]
    public class TestController : BaseController
    {
        private CloudQueue addResourceRequestQueue;
        private CloudQueue addPopulationQueue;
        private CloudQueue nightlyAttackQueue;

        public TestController()
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            addResourceRequestQueue = queueClient.GetQueueReference(QueueNames.ResourceQueue);
            addResourceRequestQueue.CreateIfNotExists();

            addPopulationQueue = queueClient.GetQueueReference(QueueNames.PopulationQueue);
            addPopulationQueue.CreateIfNotExists();

            nightlyAttackQueue = queueClient.GetQueueReference(QueueNames.NightlyAttackQueue);
            nightlyAttackQueue.CreateIfNotExists();
        }

        // GET: Test
        public ActionResult Index()
        {
            ViewBag.Energy = StatManager.GetEnergy(this.UserContext.Id, new Models.ApplicationDbContext());
            return View();
        }

        public ActionResult UseEnergyTest(int use)
        {
            this.UserContext.Energy -= use;
            this.UserContext.EnergyUpdatedTime = DateTime.Now;
            this.Context.SaveChanges();

            return RedirectToAction("Index");
        }

        //public async Task<ActionResult> AddResourcesTest(ResourceTypes type, int quantity, int delayInSeconds)
        //{
        //    // queue message with delay
        //    var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(new AddResourceMessage
        //    {
        //        Type = type,
        //        Quantity = quantity,
        //        TerritoryId = this.UserContext.TerritoryId.Value,
        //    }));
        //    await addResourceRequestQueue.AddMessageAsync(queueMessage, null, TimeSpan.FromSeconds(delayInSeconds), null, null);

        //    return RedirectToAction("Index");
        //}

        public async Task<ActionResult> AddPopulationTest(int quantity, int delayInSeconds)
        {
            // queue message with delay
            var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(new AddPopulationMessage
            {
                Population = quantity,
                TerritoryId = this.UserContext.TerritoryId.Value,
            }));
            await addPopulationQueue.AddMessageAsync(queueMessage, null, TimeSpan.FromSeconds(delayInSeconds), null, null);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> NightlyAttackTest(int delayInSeconds)
        {
            // queue message with delay
            var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(new NightlyAttackMessage
            {
                TerritoryId = this.UserContext.TerritoryId.Value,
            }));
            await nightlyAttackQueue.AddMessageAsync(queueMessage, null, TimeSpan.FromSeconds(delayInSeconds), null, null);

            return RedirectToAction("Index");
        }
    }
}