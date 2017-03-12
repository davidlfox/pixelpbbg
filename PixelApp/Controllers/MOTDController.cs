using Pixel.Common;
using PixelApp.Views.MOTD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Pixel.Common.Cloud;

namespace PixelApp.Controllers
{
    public class MOTDController : BaseController
    {
        // GET: MOTD
        [Authorize]
        public ActionResult Index()
        {
            // todo: get list of MOTD's order by descending post date
            var storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager
                .ConnectionStrings["StorageConnectionString"].ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableNames.MOTD);
            table.CreateIfNotExists();

            var query = new TableQuery<MessageOfTheDayEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "motd"));

            var vm = table.ExecuteQuery(query)
                .OrderByDescending(x => x.Posted)
                .Select(x => new MOTDViewModel
                {
                    Author = x.Author,
                    Message = x.Message,
                    Posted = x.Posted,
                })
                .ToList();

            return View(vm);
        }

        [HttpGet]
        [Authorize(Roles = Permissions.CanEditMOTD)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = Permissions.CanEditMOTD)]
        public ActionResult Create(MOTDViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var model = new MessageOfTheDayEntity
            {
                Author = vm.Author,
                Message = vm.Message,
                Posted = vm.Posted,
            };

            var op = TableOperation.Insert(model);

            var storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager
                .ConnectionStrings["StorageConnectionString"].ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableNames.MOTD);
            table.CreateIfNotExists();

            table.Execute(op);

            return RedirectToAction("Index");
        }
    }
}