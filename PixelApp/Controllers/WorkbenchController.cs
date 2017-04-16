using PixelApp.Views.Workbench.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace PixelApp.Controllers
{
    [Authorize]
    public class WorkbenchController : BaseController
    {
        // GET: Workbench
        public ActionResult Index()
        {
            var vm = new WorkbenchIndexViewModel();
            return View(vm);
        }

        [HttpGet]
        public JsonResult GetItems()
        {
            var userId = this.User.Identity.GetUserId();
            var user = this.Context.Users.Include(x => x.Items).Single(x => x.Id == userId);

            var vm = new WorkbenchViewModel(user.Items);
            // get items and affordability for the user
            vm.LoadItems(this.Context);
            vm.CivilianPopulation = user.Territory.CivilianPopulation;
            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Craft(WorkbenchCraftViewModel vm)
        {
            if (vm.ItemId <= 0)
            {
                return View("Index", new WorkbenchIndexViewModel { ErrorMessage = "You must select an item to craft." });
            }
            if(vm.Quantity <= 0)
            {
                return View("Index", new WorkbenchIndexViewModel { ErrorMessage = "You must enter a quantity greater than zero." });
            }

            // get item
            var item = this.Context.Items
                .Include(x => x.Required)
                .Single(x => x.ItemId == vm.ItemId);

            var updates = new List<Task>();

            // validate user can afford
            foreach (var ing in item.Required)
            {
                var userIngredientItem = this.UserContext.Items.FirstOrDefault(x => x.ItemId == ing.IngredientItemId);
                var required = ing.Quantity * vm.Quantity;
                if (userIngredientItem == null || userIngredientItem.Quantity < required)
                {
                    return View("Index", new WorkbenchIndexViewModel { ErrorMessage = "You have insufficient resources to craft that." });
                }
                else
                {
                    // setup task to execute
                    updates.Add(new Task(() => { userIngredientItem.Quantity -= required; }));
                }
            }

            foreach (var task in updates)
            {
                task.RunSynchronously();
            }

            // add item(s) to player
            var userItem = this.UserContext.Items.FirstOrDefault(x => x.ItemId == item.ItemId);
            if (userItem == null)
            {
                this.Context.UserItems.Add(new Models.UserItem { ItemId = item.ItemId, UserId = this.UserContext.Id, Quantity = vm.Quantity });
            }
            else
            {
                userItem.Quantity += vm.Quantity;
            }

            this.Context.SaveChanges();

            return View("Index", new WorkbenchIndexViewModel { SuccessMessage = "Great success!" });
        }
    }
}