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
    [Authorize(Roles = Permissions.CanEditBadges)]
    public class UserBadgesController : BaseController
    {
        // GET: UserBadges
        public async Task<ActionResult> Index()
        {
            var userBadges = this.Context.UserBadges.Include(u => u.Badge).Include(u => u.User);
            return View(await userBadges.ToListAsync());
        }

        // GET: UserBadges/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserBadge userBadge = await this.Context.UserBadges.FindAsync(id);
            if (userBadge == null)
            {
                return HttpNotFound();
            }
            return View(userBadge);
        }

        // GET: UserBadges/Create
        public ActionResult Create()
        {
            ViewBag.BadgeId = new SelectList(this.Context.Badges, "BadgeId", "Name");
            ViewBag.UserId = new SelectList(this.Context.Users, "Id", "UserName");
            return View();
        }

        // POST: UserBadges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "UserBadgeId,Created,UserId,BadgeId")] UserBadge userBadge)
        {
            if (ModelState.IsValid)
            {
                this.Context.UserBadges.Add(userBadge);
                await this.Context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.BadgeId = new SelectList(this.Context.Badges, "BadgeId", "Name", userBadge.BadgeId);
            ViewBag.UserId = new SelectList(this.Context.Users, "Id", "UserName", userBadge.UserId);
            return View(userBadge);
        }

        // GET: UserBadges/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserBadge userBadge = await this.Context.UserBadges.FindAsync(id);
            if (userBadge == null)
            {
                return HttpNotFound();
            }
            ViewBag.BadgeId = new SelectList(this.Context.Badges, "BadgeId", "Name", userBadge.BadgeId);
            ViewBag.UserId = new SelectList(this.Context.Users, "Id", "UserName", userBadge.UserId);
            return View(userBadge);
        }

        // POST: UserBadges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "UserBadgeId,Created,UserId,BadgeId")] UserBadge userBadge)
        {
            if (ModelState.IsValid)
            {
                this.Context.Entry(userBadge).State = EntityState.Modified;
                await this.Context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.BadgeId = new SelectList(this.Context.Badges, "BadgeId", "Name", userBadge.BadgeId);
            ViewBag.UserId = new SelectList(this.Context.Users, "Id", "UserName", userBadge.UserId);
            return View(userBadge);
        }

        // GET: UserBadges/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserBadge userBadge = await this.Context.UserBadges.FindAsync(id);
            if (userBadge == null)
            {
                return HttpNotFound();
            }
            return View(userBadge);
        }

        // POST: UserBadges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            UserBadge userBadge = await this.Context.UserBadges.FindAsync(id);
            this.Context.UserBadges.Remove(userBadge);
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
