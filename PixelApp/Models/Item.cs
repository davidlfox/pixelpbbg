using Pixel.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PixelApp.Models
{
    public class Item
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Name of item
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Description of the item
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Whether or not this is a core item e.g. raw resource, etc
        /// </summary>
        public bool IsCore { get; set; }

        /// <summary>
        /// Maximum boost at 100% utilization
        /// </summary>
        /// <example>If 10 civilians allocated to use pickaxe, 11th pickaxe has no effect</example>
        public decimal MaxBoost { get; set; }

        /// <summary>
        /// The boost type
        /// </summary>
        public BoostTypes BoostType { get; set; }

        /// <summary>
        /// Collection of items required to craft item
        /// </summary>
        public virtual ICollection<ItemIngredient> Required { get; set; }
    }
}