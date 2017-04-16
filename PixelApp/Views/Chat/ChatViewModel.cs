using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Chat
{
    public class ChatViewModel
    {
        [Required]
        [StringLength(1000)]
        public string Message { get; set; }
        public string Author { get; set; }
        public string ImageUrl { get; set; }
        public string Posted { get; set; }
    }
}