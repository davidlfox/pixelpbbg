using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Profile.Models
{
    public class ProfileViewModel
    {
        public List<BadgeSkinny> Badges { get; set; }

        public ProfileViewModel()
        {
            this.Badges = new List<BadgeSkinny>();
        }
    }
}