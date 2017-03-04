using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Models
{
    /// <summary>
    /// A user notification
    /// </summary>
    public class Note
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int NoteId { get; set; }

        /// <summary>
        /// Something like the subject line
        /// </summary>
        [Required]
        [AllowHtml]
        public string Title { get; set; }

        /// <summary>
        /// Main contents of the notification
        /// </summary>
        [Required]
        [AllowHtml]
        public string Body { get; set; }

        /// <summary>
        /// Date sent
        /// </summary>
        [Required]
        public DateTime Sent { get; set; }

        /// <summary>
        /// Whether or not the message has been read
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Whether or not the message is active i.e. not been deleted
        /// </summary>
        public bool IsActive { get; set; }
        
        /// <summary>
        /// The user notified
        /// </summary>
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        /// <summary>
        /// Navigation property for the user notified
        /// </summary>
        public virtual ApplicationUser User { get; set; }
    }
}