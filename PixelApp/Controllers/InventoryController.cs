using PixelApp.Views.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Controllers
{
    [Authorize]
    public class InventoryController : BaseController
    {
        // GET: Inventory
        public ActionResult Index()
        {
            var vm = new InventoryIndexViewModel();
            vm.LoadItems(this.Context, this.UserContext, this.UserContext.Territory.CivilianPopulation);
            return View(vm);
        }
    }
}