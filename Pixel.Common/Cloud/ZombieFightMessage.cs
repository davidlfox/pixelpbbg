using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixel.Common.Cloud
{
    public class ZombieFightMessage
    {
        public string UserId { get; set; }
        public int DeltaXp { get; set; }
        public int DeltaLife { get; set; }
        public bool IsWin { get; set; }
        public bool IsDead { get; set; }
    }
}
