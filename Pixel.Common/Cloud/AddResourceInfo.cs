using Pixel.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixel.Common.Cloud
{
    public class AddResourceInfo
    {
        public int TerritoryId { get; set; }
        public ResourceTypes Type { get; set; }
        public int Quantity { get; set; }
    }
}
