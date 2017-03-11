using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixel.Common.Cloud
{
    public class FoodForageEntity : TableEntity
    {
        public string UserId { get; set; }
        public int DeltaXp { get; set; }
        public int FoodFound { get; set; }

        public FoodForageEntity()
        {
            this.PartitionKey = TablePartitionKeys.GameEvents.FoodForages;
            this.RowKey = Guid.NewGuid().ToString();
        }
    }
}
