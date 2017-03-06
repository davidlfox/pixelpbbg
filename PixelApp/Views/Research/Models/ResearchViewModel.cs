using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Research.Models
{
    public class ResearchViewModel
    {
        public List<Technology> Technologies { get; set; }

        public List<int> ResearchedTechnologyIds { get; set; }

        public UserTechnology CurrentlyResearching { get; set; }
    }
}