using Pixel.Common.Data;
using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Services
{
    public class TerritoryFactory
    {
        private static Random rand = new Random();

        public static Territory CreateTerritory()
        {
            var terr = new Territory();

            terr.Name = "Unnamed Territory";

            terr.Players = new List<ApplicationUser>();

            terr.Type = GetRandomTerritoryType();

            terr.CivilianPopulation = 0;

            terr.Water = GetRandomResource(ResourceTypes.Water, terr.Type);
            terr.Wood = GetRandomResource(ResourceTypes.Wood, terr.Type);
            terr.Coal = GetRandomResource(ResourceTypes.Coal, terr.Type);
            terr.Stone = GetRandomResource(ResourceTypes.Stone, terr.Type);
            terr.Oil = GetRandomResource(ResourceTypes.Oil, terr.Type);
            terr.Iron = GetRandomResource(ResourceTypes.Iron, terr.Type);

            var db = new ApplicationDbContext();

            // todo: algorithm to randomly select coordinates
            var maxX = db.Territories.OrderByDescending(x => x.X).FirstOrDefault();

            if(maxX == null)
            {
                terr.X = 0;
            }
            else
            {
                // just generate X coordinate to the east of the eastmost territory
                terr.X = maxX.X + 1;
            }

            terr.Y = 0;

            return terr;
        }

        private static TerritoryTypes GetRandomTerritoryType()
        {
            return (TerritoryTypes)(rand.Next(0, 4) + 1);
        }

        private static decimal GetRandomResource(ResourceTypes resType, TerritoryTypes terrType)
        {
            var ret = 0m;

            switch (terrType)
            {
                case TerritoryTypes.Desert:
                    switch (resType)
                    {
                        case ResourceTypes.Coal:
                        case ResourceTypes.Iron:
                        case ResourceTypes.Stone:
                        case ResourceTypes.Wood:
                            ret = 0.5m;
                            break;
                        case ResourceTypes.Oil:
                            ret = 0.8m;
                            break;
                        case ResourceTypes.Water:
                            ret = 0.3m;
                            break;
                    }
                    break;
                case TerritoryTypes.Forest:
                    switch (resType)
                    {
                        case ResourceTypes.Coal:
                        case ResourceTypes.Iron:
                        case ResourceTypes.Stone:
                        case ResourceTypes.Water:
                            ret = 0.5m;
                            break;
                        case ResourceTypes.Wood:
                            ret = 0.9m;
                            break;
                        case ResourceTypes.Oil:
                            ret = 0.3m;
                            break;
                    }
                    break;
                case TerritoryTypes.Rural:
                    switch (resType)
                    {
                        case ResourceTypes.Water:
                        case ResourceTypes.Oil:
                        case ResourceTypes.Wood:
                            ret = 0.5m;
                            break;
                        case ResourceTypes.Coal:
                        case ResourceTypes.Stone:
                            ret = 0.8m;
                            break;
                        case ResourceTypes.Iron:
                            ret = 0.3m;
                            break;
                    }
                    break;
                case TerritoryTypes.Urban:
                    switch (resType)
                    {
                        case ResourceTypes.Coal:
                        case ResourceTypes.Oil:
                        case ResourceTypes.Wood:
                        case ResourceTypes.Water:
                            ret = 0.5m;
                            break;
                        case ResourceTypes.Iron:
                            ret = 0.8m;
                            break;
                        case ResourceTypes.Stone:
                            ret = 0.3m;
                            break;
                    }
                    break;
            }

            return ret;
        }
    }
}