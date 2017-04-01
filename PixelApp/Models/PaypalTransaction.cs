using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PixelApp.Models
{
    public class PaypalTransaction
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int PaypalTransactionId { get; set; }

        /// <summary>
        /// Product string
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// Unique transaction id
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Number of item purchased
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Paypal fee from purchase
        /// </summary>
        public decimal Fee { get; set; }

        /// <summary>
        /// Amount of purchase
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Payment status
        /// </summary>
        public string PaymentStatus { get; set; }

        /// <summary>
        /// Purchasing user
        /// </summary>
        [ForeignKey("User")]
        public string UserId { get; set; }

        /// <summary>
        /// Navigation property for purchasing user
        /// </summary>
        public virtual ApplicationUser User { get; set; }
    }
}