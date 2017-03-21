using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Workbench.Models
{
    public class WorkbenchCraftViewModel
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
}