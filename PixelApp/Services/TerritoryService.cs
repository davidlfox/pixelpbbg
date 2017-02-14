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
    }
}