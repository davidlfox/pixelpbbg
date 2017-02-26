using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Find.Models
{
    public class FindFoodViewModel
    {
        public bool IsSuccess { get; set; }
        public bool HasForaged { get; set; }
        public string Message { get; set; }
    }
}