using PixelApp.Views.Profile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        public ActionResult Index()
        {
            var vm = new ProfileViewModel();

            vm.Badges = this.Context.UserBadges
                .Where(x => x.UserId == this.UserContext.Id)
                .OrderByDescending(x => x.Created)
                .Take(5)
                .Select(x => new BadgeSkinny
                {
                    BadgeId = x.BadgeId,
                    Name = x.Badge.Name,
                    Description = x.Badge.Description,
                    Level = x.Badge.Level,
                    ImageUrl = x.Badge.ImageUrl,
                    BadgeType = x.Badge.BadgeType,
                    HasBadge = true,
                })
                .ToList();

            return View(vm);
        }

        // GET: Profile
        public ActionResult Badges()
        {
            var vm = new BadgesViewModel();

            vm.Badges = this.Context.Badges
                .OrderBy(x => x.BadgeType)
                .ThenBy(x => x.Level)
                .Select(x => new BadgeSkinny
                {
                    BadgeId = x.BadgeId,
                    Name = x.Name,
                    Description = x.Description,
                    Level = x.Level,
                    ImageUrl = x.ImageUrl,
                    BadgeType = x.BadgeType,
                })
                .ToList();

            // marry userbadge data to collection of badges
            var userBadges = this.Context.UserBadges.Where(x => x.UserId == this.UserContext.Id).ToList();
            foreach (var badge in vm.Badges)
            {
                badge.HasBadge = userBadges.Any(x => x.BadgeId == badge.BadgeId);
            }

            return View(vm);
        }
    }
}