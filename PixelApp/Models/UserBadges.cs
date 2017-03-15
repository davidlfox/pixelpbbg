using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PixelApp.Models
{
    public class UserBadge
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int UserBadgeId { get; set; }

        /// <summary>
        /// Date badge was conferred
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Fk to user
        /// </summary>
        [ForeignKey("User")]
        public string UserId { get; set; }

        /// <summary>
        /// Navigation property to related user
        /// </summary>
        public virtual ApplicationUser User { get; set; }

        /// <summary>
        /// Fk to badge
        /// </summary>
        [ForeignKey("Badge")]
        public int BadgeId { get; set; }

        /// <summary>
        /// Navigation property to related badge
        /// </summary>
        public virtual Badge Badge { get; set; }
    }
}