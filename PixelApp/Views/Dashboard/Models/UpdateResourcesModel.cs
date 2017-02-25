using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Dashboard.Models
{
    public class UpdateResourcesViewModel : IValidatableObject
    {
        public int WaterAllocation { get; set; }
        public int WoodAllocation { get; set; }
        public int FoodAllocation { get; set; }
        public int StoneAllocation { get; set; }
        public int OilAllocation { get; set; }
        public int IronAllocation { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (WaterAllocation + WoodAllocation + FoodAllocation + StoneAllocation + OilAllocation + IronAllocation > 100)
            {
                yield return new ValidationResult("Allocation percentages must be 100% or less.");
            }
        }
    }
}