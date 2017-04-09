using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using PixelApp.Models;
using Pixel.Common;

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
