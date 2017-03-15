using Pixel.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixel.Common.Cloud
{
    public class AddResourceMessage
    {
        public string UserId { get; set; }
        public int Water { get; set; }
        public int Wood { get; set; }
        public int Food { get; set; }
        public int Stone { get; set; }
        public int Oil { get; set; }
        public int Iron { get; set; }
    }
}
