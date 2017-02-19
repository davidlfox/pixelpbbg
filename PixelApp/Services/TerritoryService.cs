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
            , decimal coal = 0, decimal stone = 0, decimal oil = 0, decimal iron = 0)
        {
            var terr = this.context.Territories.Single(x => x.TerritoryId == territoryId);
            terr.WaterAllocation = water / 100m;
            terr.WoodAllocation = wood / 100m;
            terr.CoalAllocation = coal / 100m;
            terr.StoneAllocation = stone / 100m;
            terr.OilAllocation = oil / 100m;
            terr.IronAllocation = iron / 100m;
        }
    }
}