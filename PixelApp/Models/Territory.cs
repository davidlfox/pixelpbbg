using Pixel.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PixelApp.Models
{
    public class Territory : IValidatableObject
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int TerritoryId { get; set; }

        /// <summary>
        /// Territory name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Type of territory
        /// </summary>
        public TerritoryTypes Type { get; set; }

        // location info
        public int X { get; set; }
        public int Y { get; set; }

        /// <summary>
        /// Probability of water
        /// </summary>
        public decimal Water { get; set; }
        /// <summary>
        /// Probability of wood
        /// </summary>
        public decimal Wood { get; set; }
        /// <summary>
        /// Probability of food
        /// </summary>
        public decimal Food { get; set; }
        /// <summary>
        /// Probability of stone
        /// </summary>
        public decimal Stone { get; set; }
        /// <summary>
        /// Probability of oil
        /// </summary>
        public decimal Oil { get; set; }
        /// <summary>
        /// Probability of iron
        /// </summary>
        public decimal Iron { get; set; }

        /// <summary>
        /// Current civilian population
        /// </summary>
        public int CivilianPopulation { get; set; }

        /// <summary>
        /// Rate of population increase
        /// </summary>
        public decimal PopulationGrowthRate { get; set; }

        /// <summary>
        /// Time of last population update
        /// </summary>
        public DateTime LastPopulationUpdate { get; set; }

        /// <summary>
        /// Date of last nightly attack
        /// </summary>
        public DateTime LastNightlyAttack { get; set; }

        // percentages of civilian population to dedicate to resource collecting
        public decimal WaterAllocation { get; set; }
        public decimal WoodAllocation { get; set; }
        public decimal FoodAllocation { get; set; }
        public decimal StoneAllocation { get; set; }
        public decimal OilAllocation { get; set; }
        public decimal IronAllocation { get; set; }

        /// <summary>
        /// Time of last resource collection update
        /// </summary>
        public DateTime LastResourceCollection { get; set; }

        /// <summary>
        /// The related player(s) (for now, just one user per territory)
        /// </summary>
        public ICollection<ApplicationUser> Players { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (WaterAllocation + WoodAllocation + FoodAllocation + StoneAllocation + OilAllocation + IronAllocation > 1)
            {
                yield return new ValidationResult("Civilian allocations must be 100% or less.");
            }
        }
    }
}