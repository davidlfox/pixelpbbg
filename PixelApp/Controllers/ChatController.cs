using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Pixel.Common.Cloud;
using PixelApp.Views.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Globalization;

namespace PixelApp.Controllers
{
    [Authorize]
    public class ChatController : BaseController
    {
        // GET: Chat
        public JsonResult Index(string since)
        {
            var chatsSince = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(since))
            {
                chatsSince = DateTime.ParseExact(since, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            else
            {
                chatsSince = DateTime.Now.AddDays(-1);
            }

            var chatsSinceTicks = string.Format("{0:D19}", DateTime.MaxValue.Ticks - chatsSince.Ticks);

            // get chats since date
            var storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager
                .ConnectionStrings["StorageConnectionString"].ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableNames.Chats);
            table.CreateIfNotExists();

            var query = new TableQuery<ChatEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, TablePartitionKeys.GameEvents.Chats))
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, chatsSinceTicks));

            var chats = table.ExecuteQuery(query)
                .OrderByDescending(x => x.RowKey)
                .Select(x => new
                {
                    Author = x.Author,
                    Message = x.Message,
                    ImageUrl = x.ImageUrl,
                    Timestamp = x.Timestamp,
                })
                .ToList();

            var chatsVm = new List<ChatViewModel>();

            foreach (var chat in chats)
            {
                var ts = DateTimeOffset.Now.Subtract(chat.Timestamp);

                chatsVm.Add(new ChatViewModel
                {
                    Author = chat.Author,
                    ImageUrl = chat.ImageUrl,
                    Message = chat.Message,
                    Posted = GetUserFriendlyTimestamp(ts),
                });
            }

            return Json(new { chats = chatsVm }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Send(ChatViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }

            var userId = this.User.Identity.GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }

            var user = this.Context.Users.Single(x => x.Id == userId);

            var model = new ChatEntity
            {
                Author = user.UserName,
                ImageUrl = "//storageasagvk5xvrja2.blob.core.windows.net/assets/defaultprofile.png",
                Message = vm.Message,
            };

            var op = TableOperation.Insert(model);

            var storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager
                .ConnectionStrings["StorageConnectionString"].ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableNames.Chats);
            table.CreateIfNotExists();

            table.Execute(op);

            // get user friendly timestamp
            var ts = DateTimeOffset.Now.Subtract(model.Timestamp);
            var date = GetUserFriendlyTimestamp(ts);

            var chats = new List<ChatViewModel>();
            chats.Add(new ChatViewModel
            {
                Author = model.Author,
                Message = vm.Message,
                ImageUrl = model.ImageUrl,
                Posted = date,
            });

            return Json(new { success = true, chats = chats }, JsonRequestBehavior.DenyGet);
        }

        private string GetUserFriendlyTimestamp(TimeSpan ts)
        {
            //if (ts.Days == 1) // one day ago
            //    return "Yesterday";

            //if (ts.Days > 1) //  more than one day ago, but less than one week ago
            //    return ts.Days + " days ago";

            if (ts.Hours == 1) // An hour ago
                return "About an hour ago";

            if (ts.Hours > 1 && ts.Hours <= 24) // More than an hour ago, but less than a day ago
                return "About " + ts.Hours + " hours ago";

            if (ts.Minutes == 1)
                return "About a minute ago";

            if (ts.Minutes == 0)
                return ts.Seconds + " seconds ago";

            return ts.Minutes + " minutes ago";
        }
    }
}