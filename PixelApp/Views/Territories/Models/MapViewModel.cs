using Pixel.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Territories.Models
{
    public class MapViewModel
    {
        public List<TerritorySkinny> Territories { get; set; }

        // center location info
        public int X { get; set; }
        public int Y { get; set; }
    }
}