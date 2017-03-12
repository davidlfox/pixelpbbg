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
using PixelApp.Views.Territories.Models;
using Pixel.Common.Data;

namespace PixelApp.Controllers
{
    [Authorize(Roles = Permissions.CanEditTerritories)]
    public class TerritoriesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Territories
        public async Task<ActionResult> Index()
        {
            return View(await db.Territories.ToListAsync());
        }

        // GET: Territories/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Territory territory = await db.Territories.FindAsync(id);
            if (territory == null)
            {
                return HttpNotFound();
            }
            return View(territory);
        }

        // GET: Territories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Territories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TerritoryId,Name,Type,X,Y,Water,Wood,Food,Stone,Oil,Iron,CivilianPopulation")] Territory territory)
        {
            if (ModelState.IsValid)
            {
                db.Territories.Add(territory);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(territory);
        }

        // GET: Territories/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Territory territory = await db.Territories.FindAsync(id);
            if (territory == null)
            {
                return HttpNotFound();
            }
            return View(territory);
        }

        // POST: Territories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "TerritoryId,Name,Type,X,Y,Water,Wood,Food,Stone,Oil,Iron,CivilianPopulation")] Territory territory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(territory).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(territory);
        }

        // GET: Territories/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Territory territory = await db.Territories.FindAsync(id);
            if (territory == null)
            {
                return HttpNotFound();
            }
            return View(territory);
        }

        // POST: Territories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Territory territory = await db.Territories.FindAsync(id);
            db.Territories.Remove(territory);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public ActionResult Map()
        {
            var vm = GenMapViewModel();
            return View(vm);
        }

        public MapViewModel GenMapViewModel()
        {
            var result = this.Context.Users.Where(x => x.Id.Equals(this.UserContext.Id))
                .Select(x => new MapViewModel
                {
                    X = x.Territory.X,
                    Y = x.Territory.Y
                }).FirstOrDefault();

            result.Territories =
            this.Context.Territories.Include(x => x.Players).Where(x =>
                x.X > (result.X - 6)
                && x.X < (result.X + 6)
                && x.Y > (result.Y - 6)
                && x.Y < (result.Y + 6)).ToList()
                .Select(x => new TerritorySkinny
                 {
                     X = x.X,
                     Y = x.Y,
                     Name = x.Name,
                     UserName = x.Players.FirstOrDefault().UserName,
                     Type = x.Type
                 }).ToList();
            return result;
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
