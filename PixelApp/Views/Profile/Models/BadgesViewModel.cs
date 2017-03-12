using Pixel.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Profile.Models
{
    public class BadgesViewModel
    {
        public List<BadgeSkinny> Badges { get; set; }

        public BadgesViewModel()
        {
            this.Badges = new List<BadgeSkinny>();
        }
    }

    public class BadgeSkinny
    {
        public int BadgeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Level { get; set; }
        public bool HasBadge { get; set; }
        public BadgeTypes BadgeType { get; set; }
    }
}