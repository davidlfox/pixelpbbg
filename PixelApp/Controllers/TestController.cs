using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
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
    [Authorize]
    public class TestController : BaseController
    {
        private CloudQueue addResourceRequestQueue;

        public TestController()
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            addResourceRequestQueue = queueClient.GetQueueReference("addresources");
            addResourceRequestQueue.CreateIfNotExists();
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

        public async Task<ActionResult> AddResourcesTest(ResourceTypes type, int quantity, int delayInSeconds)
        {
            // queue message with delay
            var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(new AddResourceMessage
            {
                Type = type,
                Quantity = quantity,
                TerritoryId = this.UserContext.TerritoryId.Value,
            }));
            await addResourceRequestQueue.AddMessageAsync(queueMessage, null, TimeSpan.FromSeconds(delayInSeconds), null, null);

            return RedirectToAction("Index");
        }
    }
}