using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PixelApp.Models
{
    public class AttackLog
    {
        public AttackLog()
        {
            this.TimeOfAttack = DateTime.Now;
        }

        /// <summary>
        /// Primary key
        /// </summary>
        public int AttackLogId { get; set; }

        /// <summary>
        /// The time of the attack
        /// </summary>
        public DateTime TimeOfAttack { get; set; }

        /// <summary>
        /// Whether or not the territory was actually attacked
        /// </summary>
        public bool WasAttacked { get; set; }

        /// <summary>
        /// The attack details e.g. population killed, resources lost, etc
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The related user id
        /// </summary>
        [ForeignKey("User")]
        public string UserId { get; set; }

        /// <summary>
        /// Navigation property for related user
        /// </summary>
        public virtual ApplicationUser User { get; set; }
    }
}