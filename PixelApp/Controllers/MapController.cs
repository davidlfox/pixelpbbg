using PixelApp.Views.Map.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace PixelApp.Controllers
{
    [Authorize]
    public class MapController : BaseController
    {
        public ActionResult Index()
        {
            var vm = GenMapViewModel();
            return View(vm);
        }

        public MapViewModel GenMapViewModel()
        {
            var result = this.Context.Users.Where(x => x.Id.Equals(this.UserContext.Id))
                .Select(x => new MapViewModel
                {
                    X = x.Territory.X,
                    Y = x.Territory.Y
                }).FirstOrDefault();

            result.Territories =
            this.Context.Territories.Include(x => x.Players).Where(x =>
                x.X > (result.X - 6)
                && x.X < (result.X + 6)
                && x.Y > (result.Y - 6)
                && x.Y < (result.Y + 6)).ToList()
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