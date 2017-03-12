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
        // GET: Profile
        public ActionResult Index()
        {
            var vm = new ProfileViewModel();

            vm.Badges = this.Context.Badges
                .OrderBy(x => x.BadgeType)
                .ThenBy(x => x.Level)
                .Select(x => new BadgeSkinny
                {
                    BadgeId = x.BadgeId,
                    Name = x.Name,
                    Description = x.Description,
                    Level = x.Level,
                    ImageUrl = "//fillmurray.com/80/80",
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