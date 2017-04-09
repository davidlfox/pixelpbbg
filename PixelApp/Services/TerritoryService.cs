using Pixel.Common.Data;
using Pixel.Common.Models;
using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

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

        public ProcessResponse AttackTerritory(ApplicationUser attacker, int territoryId)
        {
            if (attacker.Energy < 50)
                return new ProcessResponse(false, "Not enough energy to Attack.");

            var response = new ProcessResponse();
            var target = context.Territories.Include(x => x.Players).FirstOrDefault(t => t.TerritoryId.Equals(territoryId));
            if (target == null)
                return new ProcessResponse(false, "Territory not found.");

            var defenderId = target.Players.First().Id;
            var defender = context.Users.Include(x => x.UserTechnologies).Include(x => x.Items).Where(x => x.Id.Equals(defenderId)).FirstOrDefault();
            var levelBoost = (attacker.Level - defender.Level) * 10;
            var offensePercent = attacker.UserTechnologies.Where(x => x.Technology.BoostTypeId == BoostTypes.Offense).Sum(x => x.Technology.BoostAmount * 100);
            var defensePercent = defender.UserTechnologies.Where(x => x.Technology.BoostTypeId == BoostTypes.Defense).Sum(x => x.Technology.BoostAmount * 100);

            var winPercent = 45 + offensePercent - defensePercent + levelBoost;
            if (winPercent < 2 && attacker.Level < defender.Level)
                return new ProcessResponse(false, "Victory is not possible");

            attacker.Energy -= 50;
            attacker.EnergyUpdatedTime = DateTime.Now;

            var baseExp = attacker.Level * 20;
            var rand = new Random();
            if (rand.Next(0, 99) + 1 < winPercent)
            {
                // Win
                response.Messages.Add("Result", "Victory");

                var xpChange = Math.Max((int)(baseExp * ((100 - winPercent) / 100)), 5);
                attacker.Experience += xpChange;
                response.Messages.Add("Experience Gain", xpChange);

                var popChange = (int)(attacker.Territory.CivilianPopulation * (rand.Next(0, 2) / 100.0));
                attacker.Territory.CivilianPopulation -= popChange;
                response.Messages.Add("Population Lost", popChange);

                var lootType = (ResourceTypes)(rand.Next(0, 6) + 1);
                var defenderItem = defender.Items.Single(x => x.ItemId == (int)lootType);
                var attackerItem = attacker.Items.Single(x => x.ItemId == (int)lootType);
                var lootAmt = (int)(defenderItem.Quantity * ((rand.Next(0, 5) + 2) / 100.0));
                attackerItem.Quantity += lootAmt;
                defenderItem.Quantity -= lootAmt;
                response.Messages.Add("Loot Taken", $"{lootAmt} {lootType.ToString()}");
            }
            else
            {
                // Loss
                response.Messages.Add("Result", "Loss");

                attacker.Experience += 5;
                response.Messages.Add("Experience Gain", 5);

                var popChange = attacker.Territory.CivilianPopulation * ((rand.Next(0, 4) + 2) / 100);
                attacker.Territory.CivilianPopulation -= popChange;
                response.Messages.Add("Population Lost", popChange);
            }

            response.IsSuccessful = true;
            return response;
        }

        public Territory GetTerritory(int x, int y)
        {
            return context.Territories.FirstOrDefault(t => t.X.Equals(x) && t.Y.Equals(y));
        }

        public IQueryable<Territory> GetTerritories()
        {
            return context.Territories;
        }
    }
}