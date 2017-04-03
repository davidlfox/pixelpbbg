using PixelApp.Views.Map.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using PixelApp.Services;
using static PixelApp.Services.TerritoryService;
using System;

namespace PixelApp.Controllers
{
    [Authorize]
    public class MapController : BaseController
    {
        /// <summary>
        /// Map view for regular map display and optional territory selection
        /// </summary>
        /// <param name="sm">Is selection mode on?</param>
        /// <returns></returns>
        public ActionResult Index(int? size = null, string mode = null)
        {
            var vm = new MapViewModel();
            var mapOptions = new MapOptions();
            if (this.UserContext.Territory != null)
            {
                mapOptions.X = this.UserContext.Territory.X;
                mapOptions.Y = this.UserContext.Territory.Y;
                mapOptions.Size = size ?? 11;
            }
            else
            {
                var ts = new TerritoryService();
                mapOptions = ts.GetFullMapOptions();
            }

            vm = GenMapViewModel(mapOptions);
            vm.Mode = mode;
            return View(vm);
        }

        public MapViewModel GenMapViewModel(MapOptions options)
        {
            var result = new MapViewModel();
            result.X = options.X;
            result.Y = options.Y;

            var reach = (int)Math.Floor(options.Size / 2.0);
            result.Reach = reach;

            result.Territories =
            this.Context.Territories.Include(x => x.Players).Where(x =>
                x.X > (options.X - reach)
                && x.X < (options.X + reach)
                && x.Y > (options.Y - reach)
                && x.Y < (options.Y + reach)).ToList()
                .Select(x => new TerritorySkinny
                {
                    X = x.X,
                    Y = x.Y,
                    Name = x.Name,
                    UserName = x.Players.FirstOrDefault().UserName,
                    UserLevel = x.Players.FirstOrDefault().Level,
                    Type = x.Type
                }).ToList();

            return result;
        }

        public ActionResult SelectTerritory(int x, int y)
        {
            if (this.UserContext.Territory != null)
                return RedirectToAction("Index");

            var territory = TerritoryFactory.CreateTerritory(x, y);
            TerritoryFactory.InitializeTerritory(territory);
            this.Context.Territories.Add(territory);
            this.UserContext.Territory = territory;
            this.Context.SaveChanges();

            return RedirectToAction("NameTerritory", "Dashboard");
        }

        [HttpGet]
        public ActionResult AttackTerritory(int xCoord, int yCoord)
        {
            var ts = new TerritoryService();
            var vm = new AttackTerritoryModel();
            var targetInfo = ts.GetTerritories().Where(x => x.X.Equals(xCoord) && x.Y.Equals(yCoord))
                .Select(x => new {
                    Name = x.Name,
                    UserName = x.Players.First().UserName,
                    Level = x.Players.First().Level,
                    TerritoryId = x.TerritoryId
                })
                .FirstOrDefault();
            if (targetInfo == null)
                return RedirectToAction("Index");

            vm.TerritoryName = targetInfo.Name;
            vm.UserName = targetInfo.UserName;
            vm.Level = targetInfo.Level;
            vm.TerritoryId = targetInfo.TerritoryId;
            return View(vm);
        }

        [HttpPost]
        public ActionResult AttackTerritory(AttackTerritoryModel vm)
        {
            var ts = new TerritoryService();
            var response = ts.AttackTerritory(this.UserContext, vm.TerritoryId);
            return View();
        }
    }
}
