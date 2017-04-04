using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Paypal.Models
{
    public class PaypalViewModel
    {
        public string UserId { get; set; }
        public string PaypalButtonId { get; set; }
        public string PaypalButtonUrl { get; set; }
        public string PaypalImageUrl { get; set; }
        public string PaypalPixelUrl { get; set; }
    }
}