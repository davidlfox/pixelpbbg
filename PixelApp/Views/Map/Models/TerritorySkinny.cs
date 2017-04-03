using Pixel.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Map.Models
{
    public class TerritorySkinny
    {
        /// <summary>
        /// The name of the territoy
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The name of the territory's owner
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The level of the territory's owner
        /// </summary>
        public byte UserLevel { get; set; }

        /// <summary>
        /// Type of territory
        /// </summary>
        public TerritoryTypes Type { get; set; }

        /// <summary>
        /// Is this territory avilable to be claimed by the next new user?
        /// </summary>
        public bool Available { get; set; }

        // location info
        public int X { get; set; }
        public int Y { get; set; }
    }
}