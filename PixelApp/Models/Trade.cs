using Pixel.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PixelApp.Models
{
    public class Trade
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int TradeId { get; set; }

        /// <summary>
        /// The quantity offered by the trader
        /// </summary>
        public int QuantityOffered { get; set; }

        /// <summary>
        /// The type offered by the trader
        /// </summary>
        public ResourceTypes TypeOffered { get; set; }

        /// <summary>
        /// The quantity requested by the trader
        /// </summary>
        public int QuantityAsked { get; set; }

        /// <summary>
        /// The type requested by the trader
        /// </summary>
        public ResourceTypes TypeAsked { get; set; }

        /// <summary>
        /// The time the post was traded
        /// </summary>
        public DateTime Posted { get; set; }

        /// <summary>
        /// Whether or not the trade is active e.g. could be canceled, accepted, etc
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The initiating trader's id
        /// </summary>
        [ForeignKey("Owner")]
        public string OwnerId { get; set; }

        /// <summary>
        /// Navigation property for the initiating trader
        /// </summary>
        public virtual ApplicationUser Owner { get; set; }

        /// <summary>
        /// The accepting user
        /// </summary>
        [ForeignKey("TradedToUser")]
        public string TradedToUserId { get; set; }

        /// <summary>
        /// Navigation property for the accepting user
        /// </summary>
        public virtual ApplicationUser TradedToUser { get; set; }

    }
}