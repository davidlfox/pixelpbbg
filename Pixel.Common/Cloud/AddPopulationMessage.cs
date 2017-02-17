using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixel.Common.Cloud
{
    public class AddPopulationMessage
    {
        public int TerritoryId { get; set; }
        public int Population { get; set; }
    }
}
