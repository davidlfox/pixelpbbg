using Microsoft.AspNet.Identity;
using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Application DB context
        /// </summary>
        protected ApplicationDbContext Context { get; set; }

        /// <summary>
        /// The user in the current request context
        /// </summary>
        protected ApplicationUser UserContext { get; set; }

        public BaseController()
        {
            this.Context = new ApplicationDbContext();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // load something like a user context
            var userId = this.User.Identity.GetUserId();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                this.UserContext = this.Context.Users.FirstOrDefault(x => x.Id == userId);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}