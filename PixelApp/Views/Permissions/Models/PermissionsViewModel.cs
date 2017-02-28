using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Permissions.Models
{
    public class PermissionsViewModel
    {
        public PermissionsViewModel()
        {
            Users = new List<ApplicationUser>();
        }

        public List<ApplicationUser> Users { get; set; }
    }
}