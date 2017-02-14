using PixelApp.Models;
using PixelApp.Services;
using PixelApp.Views.Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            var vm = new DashboardViewModel();
            return View(vm);
        }

        [HttpGet]
        public ActionResult NameTerritory()
        {
            var vm = new NameTerritoryViewModel();

            // find a territory or create new one
            var available = this.Context.Territories.Where(x => x.Players.Count == 0);

            Territory territory = null;

            // todo: put this in domain/data layer
            if (available.Any())
            {
                // get a random one
                var length = available.Count();
                var rand = new Random();
                var index = rand.Next(0, length);
                territory = available.ElementAt(index);
            }
            else
            {
                // create territory
                territory = TerritoryFactory.CreateTerritory();
                territory.Players.Add(this.UserContext);
                
                this.Context.Territories.Add(territory);
            }

            this.Context.SaveChanges();

            return View(vm);
        }

        [HttpPost]
        public ActionResult NameTerritory(NameTerritoryViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // confirm territory is owned by this player,
            // so users cant hit this action and rename a territory
            var territory = this.Context.Territories.Single(x => x.TerritoryId == this.UserContext.TerritoryId
                && x.Players.Any(y => y.Id == this.UserContext.Id));

            // todo: profanity check

            this.UserContext.Territory.Name = vm.Name;
            this.Context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}