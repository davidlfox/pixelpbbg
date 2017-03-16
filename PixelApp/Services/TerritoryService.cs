using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Services
{
    public enum Directions
    {
        North,
        South,
        East,
        West,
    }

    public class TerritoryService
    {
        private ApplicationDbContext context;

        public TerritoryService()
        {
            this.context = new ApplicationDbContext();
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        public static Dictionary<Directions, Territory> GetNeighbors(Territory territory)
        {
            var db = new ApplicationDbContext();
            var neighbors = db.Territories.Where(t =>
                   (t.X == territory.X + 1 && t.Y == territory.Y)
                || (t.X == territory.X - 1 && t.Y == territory.Y)
                || (t.X == territory.X && t.Y == territory.Y + 1)
                || (t.X == territory.X && t.Y == territory.Y - 1)
            ).ToList();

            var ret = new Dictionary<Directions, Territory>();

            foreach (var neighbor in neighbors)
            {
                if (neighbor.X == territory.X + 1)
                {
                    ret.Add(Directions.East, neighbor);
                }
                else if (neighbor.X == territory.X - 1)
                {
                    ret.Add(Directions.West, neighbor);
                }
                else if (neighbor.Y == territory.Y + 1)
                {
                    ret.Add(Directions.North, neighbor);
                }
                else if (neighbor.Y == territory.Y - 1)
                {
                    ret.Add(Directions.South, neighbor);
                }
            }

            return ret;
        }

        public void UpdateResourceAllocations(int territoryId, decimal water = 0, decimal wood = 0
            , decimal food = 0, decimal stone = 0, decimal oil = 0, decimal iron = 0)
        {
            var terr = this.context.Territories.Single(x => x.TerritoryId == territoryId);
            terr.WaterAllocation = water / 100m;
            terr.WoodAllocation = wood / 100m;
            terr.FoodAllocation = food / 100m;
            terr.StoneAllocation = stone / 100m;
            terr.OilAllocation = oil / 100m;
            terr.IronAllocation = iron / 100m;
        }

        public MapOptions GetFullMapOptions()
        {
            var result = new MapOptions();
            var minX = context.Territories.Min(x => x.X);
            var maxX = context.Territories.Max(x => x.X);
            var minY = context.Territories.Min(x => x.Y);
            var maxY = context.Territories.Max(x => x.Y);

            result.Size = Math.Max(maxX - minX, maxY - minY) + 4;
            result.X = maxX - ((maxX - minX) / 2);
            result.Y = maxY - ((maxY - minY) / 2);

            return result;
        }

        public class MapOptions
        {
            public int Size { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}