using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PixelApp.Models
{
    public class UserItem
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int UserItemId { get; set; }

        /// <summary>
        /// Number of item user owns
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Fk to owner of item
        /// </summary>
        [ForeignKey("User")]
        public string UserId { get; set; }

        /// <summary>
        /// Navigation property for owner of item
        /// </summary>
        public virtual ApplicationUser User { get; set; }

        /// <summary>
        /// Fk to item
        /// </summary>
        [ForeignKey("Item")]
        public int ItemId { get; set; }

        /// <summary>
        /// Navigation property for item
        /// </summary>
        public virtual Item Item { get; set; }
    }
}