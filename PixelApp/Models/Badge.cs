using Pixel.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Models
{
    public class Badge
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int BadgeId { get; set; }

        /// <summary>
        /// Name of badge
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of badge
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The type of badge
        /// </summary>
        public BadgeTypes BadgeType { get; set; }

        /// <summary>
        /// Experienced gain when receiving badge
        /// </summary>
        public int ExperienceGain { get; set; }

        /// <summary>
        /// Level of badge e.g. 10 Zombie kills is level 1, 100 is level 2, etc
        /// </summary>
        /// <remarks>Could be leveraged for UI</remarks>
        public int Level { get; set; }
    }
}