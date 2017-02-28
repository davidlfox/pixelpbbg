using PixelApp.Models;
using PixelApp.Services;
using PixelApp.Views.Research.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Controllers
{
    public class ResearchController : Controller
    {
        // GET: Research
        public ActionResult Index()
        {
            var ts = new TechnologyService();
            var vm = new ResearchViewModel();
            vm.Technologies = ts.GetTechnologies().ToList();
            return View(vm);
        }

        public ActionResult StartResearch(int technologyId)
        {
            var ts = new TechnologyService();
            //ts.StartResearch(technologyId);

            return Json(new { success = true });
        }
    }
}