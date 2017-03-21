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
    [Authorize(Roles = Permissions.CanEditItems)]
    public class UserItemsController : BaseController
    {
        // GET: UserItems
        public async Task<ActionResult> Index()
        {
            var userItems = this.Context.UserItems.Include(u => u.Item).Include(u => u.User);
            return View(await userItems.ToListAsync());
        }

        // GET: UserItems/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserItem userItem = await this.Context.UserItems.FindAsync(id);
            if (userItem == null)
            {
                return HttpNotFound();
            }
            return View(userItem);
        }

        // GET: UserItems/Create
        public ActionResult Create()
        {
            ViewBag.ItemId = new SelectList(this.Context.Items, "ItemId", "Name");
            ViewBag.UserId = new SelectList(this.Context.Users, "Id", "UserName");
            return View();
        }

        // POST: UserItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "UserItemId,Quantity,UserId,ItemId")] UserItem userItem)
        {
            if (ModelState.IsValid)
            {
                this.Context.UserItems.Add(userItem);
                await this.Context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ItemId = new SelectList(this.Context.Items, "ItemId", "Name", userItem.ItemId);
            ViewBag.UserId = new SelectList(this.Context.Users, "Id", "UserName", userItem.UserId);
            return View(userItem);
        }

        // GET: UserItems/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserItem userItem = await this.Context.UserItems.FindAsync(id);
            if (userItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemId = new SelectList(this.Context.Items, "ItemId", "Name", userItem.ItemId);
            ViewBag.UserId = new SelectList(this.Context.Users, "Id", "UserName", userItem.UserId);
            return View(userItem);
        }

        // POST: UserItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "UserItemId,Quantity,UserId,ItemId")] UserItem userItem)
        {
            if (ModelState.IsValid)
            {
                this.Context.Entry(userItem).State = EntityState.Modified;
                await this.Context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ItemId = new SelectList(this.Context.Items, "ItemId", "Name", userItem.ItemId);
            ViewBag.UserId = new SelectList(this.Context.Users, "Id", "UserName", userItem.UserId);
            return View(userItem);
        }

        // GET: UserItems/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserItem userItem = await this.Context.UserItems.FindAsync(id);
            if (userItem == null)
            {
                return HttpNotFound();
            }
            return View(userItem);
        }

        // POST: UserItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            UserItem userItem = await this.Context.UserItems.FindAsync(id);
            this.Context.UserItems.Remove(userItem);
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
