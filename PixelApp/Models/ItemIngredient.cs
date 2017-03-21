using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PixelApp.Models
{
    public class ItemIngredient
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int ItemIngredientId { get; set; }

        /// <summary>
        /// Quantity required
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Item being crafted
        /// </summary>
        [ForeignKey("Item")]
        public int ItemId { get; set; }

        /// <summary>
        /// Navigation property for item being created
        /// </summary>
        public virtual Item Item { get; set; }

        /// <summary>
        /// Component item i.e. the ingredient item required
        /// </summary>
        [ForeignKey("IngredientItem")]
        public int IngredientItemId { get; set; }

        /// <summary>
        /// Navigation property for the ingredient item
        /// </summary>
        public virtual Item IngredientItem { get; set; }
    }
}