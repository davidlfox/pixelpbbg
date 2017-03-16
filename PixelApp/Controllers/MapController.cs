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
        public ActionResult Index(int? size = null, bool sm = false)
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
                var fullMapOptions = ts.GetFullMapOptions();
            }

            vm = GenMapViewModel(mapOptions);
            vm.IsTerritorySelectionMode = sm;
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
                    Type = x.Type
                }).ToList();

            return result;
        }
    }
}