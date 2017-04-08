using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Pixel.Common;
using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Controllers
{
    public class ImpersonateController : BaseController
    {
        [Authorize(Roles = Permissions.CanImpersonateUsers)]
        public async Task<ActionResult> StartAsync(string userName)
        {
            var context = System.Web.HttpContext.Current;

            var originalUsername = context.User.Identity.Name;

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var impersonatedUser = await userManager.FindByNameAsync(userName);

            var impersonatedIdentity = await userManager.CreateIdentityAsync(impersonatedUser, DefaultAuthenticationTypes.ApplicationCookie);
            impersonatedIdentity.AddClaim(new Claim("UserImpersonation", "true"));
            impersonatedIdentity.AddClaim(new Claim("OriginalUsername", originalUsername));

            var authenticationManager = context.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, impersonatedIdentity);

            return RedirectToAction("Index", "Dashboard");
        }

        [Authorize]
        public async Task<ActionResult> StopAsync()
        {
            if (System.Web.HttpContext.Current.User.IsImpersonating())
            {
                var context = System.Web.HttpContext.Current;

                var originalUsername = context.User.GetOriginalUsername();

                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

                var originalUser = await userManager.FindByNameAsync(originalUsername);

                var impersonatedIdentity = await userManager.CreateIdentityAsync(originalUser, DefaultAuthenticationTypes.ApplicationCookie);
                var authenticationManager = context.GetOwinContext().Authentication;

                authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, impersonatedIdentity);
            }
            else
            {
                throw new Exception("Unable to remove impersonation because there is no impersonation");
            }

            return RedirectToAction("Index", "Dashboard");
        }
    }
}