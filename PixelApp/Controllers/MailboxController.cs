using PixelApp.Views.Mailbox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Controllers
{
    public class MailboxController : BaseController
    {
        public JsonResult Any()
        {
            var notes = this.Context.Notes.Any(x => 
                x.UserId == this.UserContext.Id
                && x.IsActive.Equals(true) 
                && x.IsRead.Equals(false));

            return Json(new { hasmail = notes }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var vm = new MailboxViewModel();

            vm.Notes = this.Context.Notes
                .Where(x => x.UserId == this.UserContext.Id)
                .Where(x => x.IsActive.Equals(true))
                .OrderByDescending(x => x.Sent)
                .Select(x => new NoteSkinny
                {
                    IsRead = x.IsRead,
                    NoteId = x.NoteId,
                    Sent = x.Sent,
                    Body = x.Body,
                    Title = x.Title,
                })
                .ToList();

            return View(vm);
        }

        public ActionResult View(int id)
        {
            var note = this.Context.Notes.Single(x => x.NoteId == id);

            if(note.UserId != this.UserContext.Id)
            {
                return HttpNotFound();
            }

            note.IsRead = true;

            return View(note);
        }

        public ActionResult Delete(int id)
        {
            var note = this.Context.Notes.Single(x => x.NoteId == id);

            if (note.UserId != this.UserContext.Id)
            {
                return HttpNotFound();
            }

            note.IsActive = false;

            return RedirectToAction("Index");
        }
    }
}