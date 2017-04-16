using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixel.Common.Cloud
{
    public class ChatEntity : TableEntity
    {
        public string Message { get; set; }
        public string Author { get; set; }
        public string ImageUrl { get; set; }

        public ChatEntity()
        {
            this.PartitionKey = TablePartitionKeys.GameEvents.Chats;
            // store a sequential row key for querying "last X interval chats"
            this.RowKey = string.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks);
        }
    }
}
