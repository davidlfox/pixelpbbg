using PixelApp.Views.Workbench.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Controllers
{
    [Authorize]
    public class WorkbenchController : BaseController
    {
        // GET: Workbench
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetItems()
        {
            var vm = new WorkbenchViewModel(this.UserContext.Items);
            // get items and affordability for the user
            vm.LoadItems(this.Context);
            vm.CivilianPopulation = this.UserContext.Territory.CivilianPopulation;
            return Json(vm, JsonRequestBehavior.AllowGet);
        }
    }
}