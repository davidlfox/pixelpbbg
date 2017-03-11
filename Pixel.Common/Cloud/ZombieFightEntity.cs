using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixel.Common.Cloud
{
    public class ZombieFightEntity : TableEntity
    {
        public string UserId { get; set; }
        public int DeltaXp { get; set; }
        public int DeltaLife { get; set; }
        public bool IsWin { get; set; }
        public bool IsDead { get; set; }

        public ZombieFightEntity()
        {
            this.PartitionKey = TablePartitionKeys.GameEvents.ZombieFights;
            this.RowKey = Guid.NewGuid().ToString();
        }
    }
}
