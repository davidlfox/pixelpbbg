using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixel.Common.Data
{
    public enum BadgeTypes : byte
    {
        /// <summary>
        /// Zombie kill counts e.g. 10, 100, etc
        /// </summary>
        ZombieKills = 1,

        /// <summary>
        /// Successful food forage counts
        /// </summary>
        FoodForages = 2,

        /// <summary>
        /// Number of subjects researched
        /// </summary>
        ResearchedSubjects = 3,

        /// <summary>
        /// Water owned currently
        /// </summary>
        WaterCount = 4,

        /// <summary>
        /// Wood owned currently
        /// </summary>
        WoodCount = 5,

        /// <summary>
        /// Food owned currently
        /// </summary>
        FoodCount = 6,

        /// <summary>
        /// Stone owned currently
        /// </summary>
        StoneCount = 7,

        /// <summary>
        /// Oil owned currently
        /// </summary>
        OilCount = 8,

        /// <summary>
        /// Iron owned currently
        /// </summary>
        IronCount = 9,

        /// <summary>
        /// Territory named
        /// </summary>
        TerritoryNamed = 10,
    }
}
