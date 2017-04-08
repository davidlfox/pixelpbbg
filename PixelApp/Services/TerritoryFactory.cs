using Pixel.Common.Cloud;
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

        public static Territory CreateTerritory(int xCoord, int yCoord)
        {
            var terr = new Territory();

            terr.Name = "Unnamed Territory";

            terr.Players = new List<ApplicationUser>();

            terr.Type = GetRandomTerritoryType();

            terr.CivilianPopulation = 0;

            terr.Water = GetRandomResource(ResourceTypes.Water, terr.Type);
            terr.Wood = GetRandomResource(ResourceTypes.Wood, terr.Type);
            terr.Food = GetRandomResource(ResourceTypes.Food, terr.Type);
            terr.Stone = GetRandomResource(ResourceTypes.Stone, terr.Type);
            terr.Oil = GetRandomResource(ResourceTypes.Oil, terr.Type);
            terr.Iron = GetRandomResource(ResourceTypes.Iron, terr.Type);
            terr.X = xCoord;
            terr.Y = yCoord;

            return terr;
        }

        /// <summary>
        /// Setup default allocations, population, etc
        /// </summary>
        /// <param name="territory">The territory to be initialized</param>
        public static void InitializeTerritory(Territory territory)
        {
            territory.CivilianPopulation = 120;
            territory.PopulationGrowthRate = 0.02m;

            territory.WaterAllocation = 0.16m;
            territory.WoodAllocation = 0.16m;
            territory.FoodAllocation = 0.16m;
            territory.StoneAllocation = 0.16m;
            territory.OilAllocation = 0.16m;
            territory.IronAllocation = 0.16m;

            // get the last hour timestamp
            var now = DateTime.Now;
            territory.LastResourceCollection = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, 0);

            // get the next midnight timestamp
            territory.LastPopulationUpdate = new DateTime(now.Year, now.Month, now.Day);
            territory.LastNightlyAttack = new DateTime(now.Year, now.Month, now.Day);
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
                        case ResourceTypes.Food:
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
                        case ResourceTypes.Food:
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
                        case ResourceTypes.Food:
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
                        case ResourceTypes.Food:
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