using PixelApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Controllers
{
    public class TestController : BaseController
    {
        // GET: Test
        public ActionResult Index()
        {
            ViewBag.Energy = StatManager.GetEnergy(this.UserContext.Id, new Models.ApplicationDbContext());
            return View();
        }

        public ActionResult UseEnergyTest(int use)
        {
            this.UserContext.Energy -= use;
            this.UserContext.EnergyUpdatedTime = DateTime.Now;
            this.Context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}