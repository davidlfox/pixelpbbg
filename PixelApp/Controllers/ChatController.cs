using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Controllers
{
    [Authorize]
    public class ChatController : BaseController
    {
        // GET: Chat
        public JsonResult Index(DateTime? chatsSince)
        {
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
    }
}