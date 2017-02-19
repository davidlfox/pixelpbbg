using Pixel.Common.Data;
using PixelApp.Models;
using PixelApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Dashboard.Models
{
    public class DashboardViewModel
    {
        public string TerritoryName { get; set; }
        public TerritoryTypes TerritoryType { get; set; }
        public int TerritoryX { get; set; }
        public int TerritoryY { get; set; }

        public string OutskirtsAppeal { get; set; }

        public List<ResourceSkinny> Resources { get; set; }

        public int CivilianPopulation { get; set; }

        public List<TerritorySkinny> Neighbors { get; set; }

        public string DailyTask { get; set; }

        public DashboardViewModel()
        {
            this.Resources = new List<ResourceSkinny>();
            this.Neighbors = new List<TerritorySkinny>();
        }
    }

    public class ResourceSkinny
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public decimal Allocation { get; set; }
    }

    public class CitizenSkinny
    {
        public string Name { get; set; }
    }

    public class TerritorySkinny
    {
        public int TerritoryId { get; set; }
        public string TerritoryName { get; set; }
        public Directions Direction { get; set; }
    }
}