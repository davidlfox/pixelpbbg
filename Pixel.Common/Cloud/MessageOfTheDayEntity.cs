using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixel.Common.Cloud
{
    public class MessageOfTheDayEntity : TableEntity
    {
        public DateTime Posted { get; set; }
        public string Message { get; set; }
        public string Author { get; set; }

        public MessageOfTheDayEntity()
        {
            this.PartitionKey = "motd";
            this.RowKey = Guid.NewGuid().ToString();
        }
    }
}
