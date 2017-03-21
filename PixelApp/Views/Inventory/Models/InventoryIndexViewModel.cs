using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Pixel.Common.Data;

namespace PixelApp.Views.Inventory.Models
{
    public class InventoryIndexViewModel
    {
        public List<InventoryItemSkinny> Items { get; set; }
        public int CivilianPopulation { get; set; }
        public Dictionary<BoostTypes, decimal> Allocations { get; set; }

        public InventoryIndexViewModel()
        {
            this.Items = new List<InventoryItemSkinny>();
            this.Allocations = new Dictionary<BoostTypes, decimal>();
        }

        public void LoadItems(ApplicationDbContext db, ApplicationUser user, int civPop)
        {
            this.CivilianPopulation = civPop;

            var userItems = db.UserItems
                .Include(x => x.Item)
                .Where(x => x.UserId == user.Id)
                .Where(x => x.Item.IsCore.Equals(false));

            this.Items = userItems
                .ToList()
                .Select(x => new InventoryItemSkinny
                {
                    UserItemId = x.UserItemId,
                    Name = x.Item.Name,
                    Description = x.Item.Description,
                    Quantity = x.Quantity,
                    BoostType = x.Item.BoostType.ToString(),
                    MaxBoost = x.Item.MaxBoost,
                })
                .ToList();

            this.Allocations.Add(BoostTypes.Water, user.Territory.WaterAllocation);
            this.Allocations.Add(BoostTypes.Food, user.Territory.FoodAllocation);
            this.Allocations.Add(BoostTypes.Wood, user.Territory.WoodAllocation);
            this.Allocations.Add(BoostTypes.Stone, user.Territory.StoneAllocation);
            this.Allocations.Add(BoostTypes.Oil, user.Territory.OilAllocation);
            this.Allocations.Add(BoostTypes.Iron, user.Territory.IronAllocation);

        }
    }

    public class InventoryItemSkinny
    {
        public int UserItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal MaxBoost { get; set; }
        public string BoostType { get; set; }
    }
}