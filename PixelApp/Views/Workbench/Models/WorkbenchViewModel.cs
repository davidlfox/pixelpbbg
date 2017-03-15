using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;

namespace PixelApp.Views.Workbench.Models
{
    public class WorkbenchViewModel
    {
        public List<ItemSkinny> Items { get; set; }
        public int CivilianPopulation { get; set; }
        public SelectList ItemSelectList { get; set; }
        private List<UserItem> userItems { get; set; }

        public WorkbenchViewModel(IEnumerable<UserItem> userItems)
        {
            this.Items = new List<ItemSkinny>();
            this.userItems = userItems.ToList();
        }

        public void LoadItems(ApplicationDbContext db)
        {
            // load all items from db
            this.Items = db.Items
                .Include(x => x.Required)
                .Where(x => x.IsCore.Equals(false)) // no point in crafting core items
                .Select(x => new ItemSkinny
                {
                    Name = x.Name,
                    Description = x.Description,
                    ItemId = x.ItemId,
                    Ingredients = x.Required.Select(y => new ItemIngredientSkinny
                    {
                        Name = y.IngredientItem.Name,
                        ItemIngredientId = y.IngredientItemId,
                        Quantity = y.Quantity,
                    }).ToList(),
                })
                .ToList();

            // determine affordability for each item
            foreach (var item in this.Items)
            {
                var affordableQtys = new List<int>();
                // quantities for each ingredient and take smallest
                foreach (var ingredient in item.Ingredients)
                {
                    ingredient.ImageUrl = $"//storageasagvk5xvrja2.blob.core.windows.net/assets/{ingredient.Name.ToLower()}.png";
                    var userItem = this.userItems.FirstOrDefault(x => x.ItemId == ingredient.ItemIngredientId);
                    if (userItem != null)
                    {
                        affordableQtys.Add(userItem.Quantity / ingredient.Quantity);
                    }
                }

                item.CanAffordQuantity = affordableQtys.Min();
            }
        }
    }

    public class ItemSkinny
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CanAffordQuantity { get; set; }
        public List<ItemIngredientSkinny> Ingredients { get; set; }
    }

    public class ItemIngredientSkinny
    {
        public int Quantity { get; set; }
        public string Name { get; set; }
        public int ItemIngredientId { get; set; }
        public string ImageUrl { get; set; }
    }
}