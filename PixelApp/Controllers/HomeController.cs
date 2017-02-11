using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            if(this.UserContext == null)
            {
                return View("HomePage");
            }
            else
            {
                return View();
            }
        }

        // todo: delete me
        public ActionResult HomePage2()
        {
            return View();
        }

        // todo: delete me
        public ActionResult HomePage3()
        {
            return View();
        }

        // todo: delete me
        public ActionResult HomePage4()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}