using Pixel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Controllers
{
    [Authorize(Roles = Permissions.CanImpersonateUsers)]
    public class ImpersonateController : BaseController
    {
        // GET: Impersonate
        public ActionResult Index()
        {
            return View();
        }
    }
}