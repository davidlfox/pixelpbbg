using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PixelApp.Models
{
    public class Territory// : IValidatableObject
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
        /// Probability of coal
        /// </summary>
        public decimal Coal { get; set; }
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

        //// percentages of civilian population to dedicate to resource collecting
        //public decimal WaterAllocation { get; set; }
        //public decimal WoodAllocation { get; set; }
        //public decimal CoalAllocation { get; set; }
        //public decimal StoneAllocation { get; set; }
        //public decimal OilAllocation { get; set; }
        //public decimal IronAllocation { get; set; }

        /// <summary>
        /// The related player(s) (for now, just one user per territory)
        /// </summary>
        public ICollection<ApplicationUser> Players { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (WaterAllocation + WoodAllocation + CoalAllocation + StoneAllocation + OilAllocation + IronAllocation > 1)
        //    {
        //        yield return new ValidationResult("Civilian allocations must be 100% or less.");
        //    }
        //}
    }

    public enum TerritoryTypes : byte
    {
        Urban = 1,
        Rural = 2,
        Forest = 3,
        Desert = 4,
    }
}