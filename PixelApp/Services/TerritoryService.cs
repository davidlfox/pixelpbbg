using Pixel.Common.Data;
using Pixel.Common.Models;
using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;

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
            if (winPercent < 2)
                return new ProcessResponse(false, "Victory is not possible");

            attacker.Energy -= 50;
            attacker.EnergyUpdatedTime = DateTime.Now;

            var rand = new Random();

            // Population change calculations
            var popChangeWin = (int)(attacker.Territory.CivilianPopulation * (rand.Next(0, 2) / 100.0));
            var popChangeLoss = (int)(attacker.Territory.CivilianPopulation * ((rand.Next(0, 4) + 2) / 100.0));

            // Base xp is a function of level and a random multiplier of 50% to 150% so the numbers aren't all the same
            var baseExp = (int)(attacker.Level * 20 * ((rand.Next(0, 101) + 50) / 100.0));
            var defenderMessages = new Dictionary<string, object>();
            if (rand.Next(0, 99) + 1 < winPercent)
            {
                // Win
                response.Messages.Add("Result", "Victory");
                defenderMessages.Add("Result", "Loss");

                var xpChange = Math.Max((int)(baseExp * ((100 - winPercent) / 100)), 5);
                attacker.Experience += xpChange;
                response.Messages.Add("Experience Gain", xpChange);

                // Attacker Population Change
                attacker.Territory.CivilianPopulation -= popChangeWin;

                response.Messages.Add("Population Lost", popChangeWin);
                defenderMessages.Add("Enemies Killed", popChangeWin);

                // Defender Population Change
                var defenderPopulationLosses = (int)(popChangeLoss * .5);
                defender.Territory.CivilianPopulation -= defenderPopulationLosses;

                response.Messages.Add("Enemies Killed", defenderPopulationLosses);
                defenderMessages.Add("Population Lost", defenderPopulationLosses);

                // Looting values
                var lootType = (ResourceTypes)(rand.Next(0, 6) + 1);
                var defenderItem = defender.Items.Single(x => x.ItemId == (int)lootType);
                var attackerItem = attacker.Items.Single(x => x.ItemId == (int)lootType);
                var lootAmt = (int)(defenderItem.Quantity * ((rand.Next(0, 5) + 2) / 100.0));
                attackerItem.Quantity += lootAmt;
                defenderItem.Quantity -= lootAmt;
                response.Messages.Add("Loot Taken", $"{lootAmt} {lootType.ToString()}");
                defenderMessages.Add("Resources Lost", $"{lootAmt} {lootType.ToString()}");
            }
            else
            {
                // Loss
                response.Messages.Add("Result", "Loss");
                defenderMessages.Add("Result", "Victory");

                // Experience Gains
                attacker.Experience += 5;
                response.Messages.Add("Experience Gain", 5);
                var defenderXPChange = Math.Max((int)(baseExp * ((winPercent) / 100)), 5);
                defender.Experience += defenderXPChange;
                defenderMessages.Add("Experience Gain", defenderXPChange);

                // Attacker Population Change
                attacker.Territory.CivilianPopulation -= popChangeLoss;
                response.Messages.Add("Population Lost", popChangeLoss);
                defenderMessages.Add("Enemies Killed", popChangeLoss);

                // Defender Population Change
                var defenderPopulationLosses = (int)(popChangeWin * .5);
                defender.Territory.CivilianPopulation -= defenderPopulationLosses;
                response.Messages.Add("Enemies Killed", defenderPopulationLosses);
                defenderMessages.Add("Population Lost", defenderPopulationLosses);
            }

            // floor population at 100 people
            if (attacker.Territory.CivilianPopulation < 100)
            {
                attacker.Territory.CivilianPopulation = 100;
            }
            if (defender.Territory.CivilianPopulation < 100)
            {
                defender.Territory.CivilianPopulation = 100;
            }

            // Send result messgages
            var sb = new StringBuilder();
            sb.Append($"Here are the results of your recent attack against {defender.Territory.Name}.<br />");
            foreach (var msg in response.Messages)
                sb.Append($"{msg.Key}: <strong>{msg.Value.ToString()}</strong><br />");
            var attackerNote = CommunicationService.CreateNotification(
                attacker.Id,
                response.Messages.ContainsKey("Result") && response.Messages["Result"].ToString() == "Victory" ? "You Triumphed Over Your Enemy" : "You Suffered a Humiliating Defeat",
                sb.ToString()               
            );

            sb = new StringBuilder();
            sb.Append($"Here are the results from the attack by your neighbor {attacker.Territory.Name}.<br />");
            foreach (var msg in defenderMessages)
                sb.Append($"{msg.Key}: <strong>{msg.Value.ToString()}</strong><br />");
            var defenderNote = CommunicationService.CreateNotification(
                defender.Id,
                response.Messages.ContainsKey("Result") && response.Messages["Result"].ToString() == "Victory" ? "You Successfully Defended Your Territory" : "You Received a Thorough Beating From an Enemy",
                sb.ToString()
            );

            context.Notes.Add(attackerNote);
            context.Notes.Add(defenderNote);

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