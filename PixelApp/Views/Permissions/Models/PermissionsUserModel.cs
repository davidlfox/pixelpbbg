using Microsoft.AspNet.Identity.EntityFramework;
using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Permissions.Models
{
    public class PermissionsUserModel
    {
        public ApplicationUser User { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }
}