﻿using Microsoft.AspNet.Identity;
using PixelApp.Models;
using PixelApp.Services;
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
                // load vitals
                this.UserContext = this.Context.Users.Single(x => x.Id == userId);
                ViewBag.Username = this.UserContext.UserName;
                ViewBag.Level = this.UserContext.Level;
                ViewBag.Life = StatManager.GetLife(userId, this.Context, false);
                ViewBag.MaxLife = this.UserContext.MaxLife;
                ViewBag.Energy = StatManager.GetEnergy(userId, this.Context, false);
                ViewBag.MaxEnergy = this.UserContext.MaxEnergy;

                this.Context.SaveChanges();
            }

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // todo: explicitly commit here?
            base.OnActionExecuted(filterContext);
        }
    }
}