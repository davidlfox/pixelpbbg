using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PixelApp.Models;
using Pixel.Common;

namespace PixelApp.Controllers
{
    [Authorize(Roles = Permissions.CanEditNotes)]
    public class NotesController : BaseController
    {
        // GET: Notes
        public async Task<ActionResult> Index()
        {
            var notes = this.Context.Notes.Include(n => n.User);
            return View(await notes.ToListAsync());
        }

        // GET: Notes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = await this.Context.Notes.FindAsync(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // GET: Notes/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(this.Context.Users, "Id", "UserName");
            return View();
        }

        // POST: Notes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "NoteId,Title,Body,Sent,IsRead,IsActive,UserId")] Note note)
        {
            if (ModelState.IsValid)
            {
                this.Context.Notes.Add(note);
                await this.Context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(this.Context.Users, "Id", "UserName", note.UserId);
            return View(note);
        }

        // GET: Notes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = await this.Context.Notes.FindAsync(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(this.Context.Users, "Id", "UserName", note.UserId);
            return View(note);
        }

        // POST: Notes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "NoteId,Title,Body,Sent,IsRead,IsActive,UserId")] Note note)
        {
            if (ModelState.IsValid)
            {
                this.Context.Entry(note).State = EntityState.Modified;
                await this.Context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(this.Context.Users, "Id", "UserName", note.UserId);
            return View(note);
        }

        // GET: Notes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = await this.Context.Notes.FindAsync(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Note note = await this.Context.Notes.FindAsync(id);
            this.Context.Notes.Remove(note);
            await this.Context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
