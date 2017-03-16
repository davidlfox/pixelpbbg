using Pixel.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Map.Models
{
    public class MapViewModel
    {
        // List of skinny territory objects which are inhabited and lie within the current view
        public List<TerritorySkinny> Territories { get; set; }

        // center location info
        public int X { get; set; }
        public int Y { get; set; }

        // Is the map allowing for territory selection
        public bool IsTerritorySelectionMode { get; set; }

        public int Reach { get; set; }
    }
}