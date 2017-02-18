using Pixel.Common.Cloud;
using PixelApp.Models;
using PixelApp.Services;
using PixelApp.Views.Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var terr = this.UserContext.Territory;
            vm.TerritoryName = terr.Name;
            vm.OutskirtsAppeal = "Good";

            vm.Resources.Add(new ResourceSkinny { Name = "Water", Count = this.UserContext.Water });
            vm.Resources.Add(new ResourceSkinny { Name = "Wood", Count = this.UserContext.Wood });
            vm.Resources.Add(new ResourceSkinny { Name = "Coal", Count = this.UserContext.Coal });
            vm.Resources.Add(new ResourceSkinny { Name = "Stone", Count = this.UserContext.Stone });
            vm.Resources.Add(new ResourceSkinny { Name = "Oil", Count = this.UserContext.Oil });
            vm.Resources.Add(new ResourceSkinny { Name = "Iron", Count = this.UserContext.Iron });

            vm.CivilianPopulation = terr.CivilianPopulation;
            vm.TerritoryType = terr.Type;
            vm.TerritoryX = terr.X;
            vm.TerritoryY = terr.Y;

            vm.Neighbors = TerritoryService.GetNeighbors(this.UserContext.Territory)
                .Select(x => new TerritorySkinny
                {
                    Direction = x.Key,
                    TerritoryId = x.Value.TerritoryId,
                    TerritoryName = x.Value.Name,
                }).ToList();

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

            TerritoryFactory.InitializeTerritory(territory);

            this.UserContext.Territory.Name = vm.Name;
            this.Context.SaveChanges();

            return RedirectToAction("Index");
        }

        // hack: this will setup queue messages for existing users who didn't have events initialized when naming their territory
        // todo: make this admin accessible only
        [HttpGet]
        public ActionResult InitializeQueuesForExistingPlayers()
        {
            // find users that have never been queued for resource/population growth
            var uninitializedTerritories = this.Context.Territories.Where(x => x.LastPopulationUpdate == null);

            // queue messages for these people
            var qm = new QueueManager();
            foreach (var user in uninitializedTerritories)
            {
                qm.QueuePopulation(user.TerritoryId, 1);
            }
            
            // do the same for resources
            var uninitializedResources = this.Context.Territories.Where(x => x.LastResourceCollectionDate == null);

            foreach (var user in uninitializedResources)
            {
                qm.QueueResourceCollection(user.TerritoryId, 1);
            }

            return RedirectToAction("Index");
        }
    }
}