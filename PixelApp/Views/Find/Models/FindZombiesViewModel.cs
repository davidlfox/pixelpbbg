using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Find.Models
{
    public class FindZombiesViewModel
    {
        public bool ShowBattle { get; set; }
        public bool IsWin { get; set; }
        public bool IsDead { get; set; }
        public string BattleText { get; set; }
        public string ResourceText { get; set; }
    }
}