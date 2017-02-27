using Pixel.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PixelApp.Models
{
    public class Technology
    {
        [Key]
        public int TechnologyId { get; set; }

        //[ForeignKey("TechnologyId")]
        //public int? PreRequisiteId { get; set; }

        //virtual public Technology PreRequisite { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public TechnologyTypes TechnologyTypeId { get; set; }

        public ResourceTypes ResourceCostTypeId { get; set; }

        public int ResourceCost { get; set; }

        public BoostTypes BoostTypeId { get; set; }

        public decimal BoostAmount { get; set; }
    }
}