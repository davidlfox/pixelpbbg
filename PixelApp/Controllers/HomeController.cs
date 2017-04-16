using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PixelApp.Models;
using PixelApp.Views.Home.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Pixel.Common.Data;

namespace PixelApp.Controllers
{
    public class HomeController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index()
        {
            if(this.UserContext == null)
            {
                return View("HomePage");
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    LastLoginDate = DateTime.Now,
                    Energy = 100,
                    MaxEnergy = 100,
                    Life = 100,
                    MaxLife = 100,
                    Level = 1,
                };

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // start user off with 50 of each core item
                    this.Context.UserItems.Add(new UserItem { ItemId = (int)ResourceTypes.Water, UserId = user.Id, Quantity = 50, });
                    this.Context.UserItems.Add(new UserItem { ItemId = (int)ResourceTypes.Food, UserId = user.Id, Quantity = 50, });
                    this.Context.UserItems.Add(new UserItem { ItemId = (int)ResourceTypes.Wood, UserId = user.Id, Quantity = 50, });
                    this.Context.UserItems.Add(new UserItem { ItemId = (int)ResourceTypes.Stone, UserId = user.Id, Quantity = 50, });
                    this.Context.UserItems.Add(new UserItem { ItemId = (int)ResourceTypes.Oil, UserId = user.Id, Quantity = 50, });
                    this.Context.UserItems.Add(new UserItem { ItemId = (int)ResourceTypes.Iron, UserId = user.Id, Quantity = 50, });

                    this.Context.SaveChanges();

                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("ReconfirmEmail", "Account", new { id = user.Id });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View("HomePage", model);
        }

        [Authorize]
        public ActionResult Users(int page = 1)
        {
            var vm = new UsersViewModel();
            vm.PageSize = 10;

            var userList = this.Context.Users
                .Include(x => x.Territory)
                .Where(x => x.TerritoryId.HasValue.Equals(true));

            vm.UserCount = userList.Count();

            var users = userList
                .OrderByDescending(x => x.LastLoginDate)
                .Skip((page - 1) * vm.PageSize)
                .Take(vm.PageSize)
                .ToList();

            if (users.Any())
            {
                vm.Users = users
                    .Select(x => new UserSkinny
                    {
                        Id = x.Id,
                        TerrainType = x.Territory.Type.ToString(),
                        Username = x.UserName,
                        TerritoryName = x.Territory.Name,
                        X = x.Territory.X,
                        Y = x.Territory.Y,
                        Level = Services.StatManager.GetLevel(x.Id, this.Context),
                    })
                    .ToList();
            }

            vm.Page = page;
            vm.PageCount = vm.UserCount / vm.PageSize + (vm.UserCount % vm.PageSize > 0 ? 1 : 0);
            vm.ShowNextButton = page * vm.PageSize < vm.UserCount;

            return View(vm);
        }

        [AllowAnonymous]
        public ActionResult Robots()
        {
            Response.ContentType = "text/plain";
            return View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}