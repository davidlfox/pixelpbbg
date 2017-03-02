using Pixel.Common.Data;
using PixelApp.Models;
using PixelApp.Services;
using PixelApp.Views.MOTD.Models;
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

        public List<AttackLogSkinny> RecentAttacks { get; set; }

        public MOTDViewModel MOTD { get; set; }

        public DashboardViewModel()
        {
            this.Resources = new List<ResourceSkinny>();
            this.Neighbors = new List<TerritorySkinny>();
            this.RecentAttacks = new List<AttackLogSkinny>();
            this.MOTD = new MOTDViewModel();
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

    public class AttackLogSkinny
    {
        public string Message { get; set; }
        public bool WasAttacked { get; set; }
        public DateTime TimeOfAttack { get; set; }
    }
}