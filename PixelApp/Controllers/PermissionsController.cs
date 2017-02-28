using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Pixel.Common;
using PixelApp.Models;
using PixelApp.Views.Permissions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Controllers
{
    public class PermissionsController : BaseController
    {
        [Authorize(Roles = Permissions.CanEditPermissions)]
        public ActionResult Index()
        {
            var db = new ApplicationDbContext();

            var vm = new PermissionsViewModel();
            vm.Users = db.Users.ToList();

            return View(vm);
        }

        public ActionResult UserDetail(string id)
        {
            var db = new ApplicationDbContext();

            var vm = new PermissionsUserModel();
            vm.User = db.Users.FirstOrDefault(x => x.Id == id);
            vm.Roles = db.Roles.ToList();

            if (vm.User == null)
            {
                ViewBag.errorMessage = "User not found";
                return View("Error");
            }

            return View(vm);
        }

        public ActionResult UpdatePermission(string userId, string roleName, bool give)
        {
            var db = new ApplicationDbContext();
            var store = new UserStore<ApplicationUser>(db);
            var manager = new UserManager<ApplicationUser>(store);

            if (give)
            {
                manager.AddToRole(userId, roleName);
            }
            else
            {
                manager.RemoveFromRole(userId, roleName);
            }

            return RedirectToAction("UserDetail", new { id = userId });
        }
    }
}