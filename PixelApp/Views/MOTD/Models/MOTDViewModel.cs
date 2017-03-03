using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PixelApp.Views.MOTD.Models
{
    public class MOTDViewModel
    {
        public string Author { get; set; }

        [Required]
        public DateTime Posted { get; set; }

        [Required]
        public string Message { get; set; }
    }
}