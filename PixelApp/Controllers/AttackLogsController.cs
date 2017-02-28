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
    [Authorize(Roles = Permissions.CanEditAttackLogs)]
    public class AttackLogsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AttackLogs
        public async Task<ActionResult> Index()
        {
            var attackLogs = db.AttackLogs.Include(a => a.User);
            return View(await attackLogs.ToListAsync());
        }

        // GET: AttackLogs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttackLog attackLog = await db.AttackLogs.FindAsync(id);
            if (attackLog == null)
            {
                return HttpNotFound();
            }
            return View(attackLog);
        }

        // GET: AttackLogs/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: AttackLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AttackLogId,TimeOfAttack,WasAttacked,Message,UserId")] AttackLog attackLog)
        {
            if (ModelState.IsValid)
            {
                db.AttackLogs.Add(attackLog);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", attackLog.UserId);
            return View(attackLog);
        }

        // GET: AttackLogs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttackLog attackLog = await db.AttackLogs.FindAsync(id);
            if (attackLog == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", attackLog.UserId);
            return View(attackLog);
        }

        // POST: AttackLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AttackLogId,TimeOfAttack,WasAttacked,Message,UserId")] AttackLog attackLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attackLog).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", attackLog.UserId);
            return View(attackLog);
        }

        // GET: AttackLogs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttackLog attackLog = await db.AttackLogs.FindAsync(id);
            if (attackLog == null)
            {
                return HttpNotFound();
            }
            return View(attackLog);
        }

        // POST: AttackLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            AttackLog attackLog = await db.AttackLogs.FindAsync(id);
            db.AttackLogs.Remove(attackLog);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
