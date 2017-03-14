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
    public class ItemIngredientsController : BaseController
    {
        // GET: ItemIngredients
        public async Task<ActionResult> Index()
        {
            var itemIngredients = this.Context.ItemIngredients.Include(i => i.IngredientItem).Include(i => i.Item);
            return View(await itemIngredients.ToListAsync());
        }

        // GET: ItemIngredients/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemIngredient itemIngredient = await this.Context.ItemIngredients.FindAsync(id);
            if (itemIngredient == null)
            {
                return HttpNotFound();
            }
            return View(itemIngredient);
        }

        // GET: ItemIngredients/Create
        public ActionResult Create()
        {
            ViewBag.IngredientItemId = new SelectList(this.Context.Items, "ItemId", "Name");
            ViewBag.ItemId = new SelectList(this.Context.Items, "ItemId", "Name");
            return View();
        }

        // POST: ItemIngredients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ItemIngredientId,Quantity,ItemId,IngredientItemId")] ItemIngredient itemIngredient)
        {
            if (ModelState.IsValid)
            {
                this.Context.ItemIngredients.Add(itemIngredient);
                await this.Context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IngredientItemId = new SelectList(this.Context.Items, "ItemId", "Name", itemIngredient.IngredientItemId);
            ViewBag.ItemId = new SelectList(this.Context.Items, "ItemId", "Name", itemIngredient.ItemId);
            return View(itemIngredient);
        }

        // GET: ItemIngredients/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemIngredient itemIngredient = await this.Context.ItemIngredients.FindAsync(id);
            if (itemIngredient == null)
            {
                return HttpNotFound();
            }
            ViewBag.IngredientItemId = new SelectList(this.Context.Items, "ItemId", "Name", itemIngredient.IngredientItemId);
            ViewBag.ItemId = new SelectList(this.Context.Items, "ItemId", "Name", itemIngredient.ItemId);
            return View(itemIngredient);
        }

        // POST: ItemIngredients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ItemIngredientId,Quantity,ItemId,IngredientItemId")] ItemIngredient itemIngredient)
        {
            if (ModelState.IsValid)
            {
                this.Context.Entry(itemIngredient).State = EntityState.Modified;
                await this.Context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IngredientItemId = new SelectList(this.Context.Items, "ItemId", "Name", itemIngredient.IngredientItemId);
            ViewBag.ItemId = new SelectList(this.Context.Items, "ItemId", "Name", itemIngredient.ItemId);
            return View(itemIngredient);
        }

        // GET: ItemIngredients/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemIngredient itemIngredient = await this.Context.ItemIngredients.FindAsync(id);
            if (itemIngredient == null)
            {
                return HttpNotFound();
            }
            return View(itemIngredient);
        }

        // POST: ItemIngredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ItemIngredient itemIngredient = await this.Context.ItemIngredients.FindAsync(id);
            this.Context.ItemIngredients.Remove(itemIngredient);
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
