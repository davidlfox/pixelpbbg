using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Research.Models
{
    public class TechDetailViewModel
    {
        public Technology Technology { get; set; }

        public UserTechnologyStatusTypes? StatusId { get; set; }
    }
}