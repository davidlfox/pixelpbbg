using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Dashboard.Models
{
    public class NameTerritoryViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}