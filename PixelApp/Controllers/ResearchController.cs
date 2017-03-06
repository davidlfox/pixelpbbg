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
    public class ResearchController : BaseController
    {
        // GET: Research
        public ActionResult Index()
        {
            var ts = new TechnologyService();
            var vm = new ResearchViewModel();
            vm.Technologies = ts.GetTechnologies().ToList();
            vm.CurrentlyResearching = ts.GetPendingReserch(this.UserContext.Id);
            vm.ResearchedTechnologyIds = ts.GetResearchedTechnologyIds(this.UserContext.Id);
            return View(vm);
        }

        public ActionResult TechDetail(int technologyId)
        {
            var ts = new TechnologyService();
            var vm = new TechDetailViewModel() { Technology = ts.GetById(technologyId) };
            vm.StatusId = ts.GetUserTechnologies().Where(x => x.UserId.Equals(this.UserContext.Id) && x.TechnologyId.Equals(technologyId)).Select(x => (UserTechnologyStatusTypes?)x.StatusId).FirstOrDefault();
            return View(vm);
        }

        [HttpPost]
        public ActionResult StartResearch(int technologyId)
        {
            var ts = new TechnologyService();

            // Check for pending Research
            var pending = ts.GetPendingReserch(this.UserContext.Id);
            if (pending != null)
                return Json(new { success = false, message = string.Format("There is research currently pending ({0}) which must finish before starting new research.", pending.Technology.Name) }, JsonRequestBehavior.DenyGet);

            // Start Researching
            var response = ts.StartResearch(technologyId, this.UserContext.Id);
            if (response.IsSuccessful)
                ts.SaveChanges();

            return Json(new { success = response.IsSuccessful, message = response.Messages["Message"].ToString() }, JsonRequestBehavior.DenyGet);
        }
    }
}