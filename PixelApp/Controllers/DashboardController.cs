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

            vm.Resources.Add(new ResourceSkinny { Name = "Water", Count = this.UserContext.Water, Allocation = terr.WaterAllocation });
            vm.Resources.Add(new ResourceSkinny { Name = "Wood", Count = this.UserContext.Wood, Allocation = terr.WoodAllocation });
            vm.Resources.Add(new ResourceSkinny { Name = "Food", Count = this.UserContext.Food, Allocation = terr.FoodAllocation });
            vm.Resources.Add(new ResourceSkinny { Name = "Stone", Count = this.UserContext.Stone, Allocation = terr.StoneAllocation });
            vm.Resources.Add(new ResourceSkinny { Name = "Oil", Count = this.UserContext.Oil, Allocation = terr.OilAllocation });
            vm.Resources.Add(new ResourceSkinny { Name = "Iron", Count = this.UserContext.Iron, Allocation = terr.IronAllocation });

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

            vm.RecentAttacks = this.Context.AttackLogs
                .Where(x => x.UserId == this.UserContext.Id)
                .OrderByDescending(x => x.TimeOfAttack)
                .Take(3)
                .Select(x => new AttackLogSkinny
                {
                    Message = x.Message,
                    WasAttacked = x.WasAttacked,
                    TimeOfAttack = x.TimeOfAttack,
                })
                .ToList();

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

        public JsonResult UpdateResourceAllocations(UpdateResourcesViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }

            var ts = new TerritoryService();
            ts.UpdateResourceAllocations(this.UserContext.TerritoryId.Value, vm.WaterAllocation, vm.WoodAllocation
                , vm.FoodAllocation, vm.StoneAllocation, vm.OilAllocation, vm.IronAllocation);
            try
            {
                ts.SaveChanges();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Json(new { success = true }, JsonRequestBehavior.DenyGet);
        }

        // hack: this will setup queue messages for existing users who didn't have events initialized when naming their territory
        // todo: make this admin accessible only
        //[HttpGet]
        //public ActionResult InitializeQueuesForExistingPlayers()
        //{
        //    // find users that have never been queued for resource/population growth
        //    var uninitializedTerritories = this.Context.Territories.Where(x => x.LastPopulationUpdate == null);

        //    // queue messages for these people
        //    var qm = new QueueManager();
        //    foreach (var user in uninitializedTerritories)
        //    {
        //        qm.QueuePopulation(user.TerritoryId, 1);
        //    }

        //    // do the same for resources
        //    var uninitializedResources = this.Context.Territories.Where(x => x.LastResourceCollectionDate == null);

        //    foreach (var user in uninitializedResources)
        //    {
        //        qm.QueueResourceCollection(user.TerritoryId, 1);
        //    }

        //    return RedirectToAction("Index");
        //}

        //public ActionResult TestInitNightly()
        //{
        //    // find users without attack logs
        //    var territories = this.Context.Users
        //        .Where(x => x.Id == this.UserContext.Id)
        //        .Select(x => x.TerritoryId);

        //    var territory = this.UserContext.Territory;

        //    var qm = new QueueManager();
        //    qm.QueueNightlyAttack(territory.TerritoryId, 1);

        //    return RedirectToAction("Index");
        //}

        public ActionResult InitializeNightlyAttackQueuesForExistingPlayers()
        {
            // find users without attack logs
            var territories = this.Context.Users
                .Where(x => x.TerritoryId.HasValue && x.AttackLogs.Count == 0)
                .Select(x => x.TerritoryId);

            var qm = new QueueManager();
            foreach (var territoryId in territories)
            {
                qm.QueueNightlyAttack(territoryId.Value);
            }

            return RedirectToAction("Index");
        }
    }
}