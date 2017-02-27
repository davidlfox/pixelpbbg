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

namespace PixelApp.Controllers
{
    [Authorize]
    public class TradesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Trades
        public async Task<ActionResult> Index()
        {
            var trades = db.Trades.Include(t => t.Owner).Include(t => t.TradedToUser);
            return View(await trades.ToListAsync());
        }

        // GET: Trades/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trade trade = await db.Trades.FindAsync(id);
            if (trade == null)
            {
                return HttpNotFound();
            }
            return View(trade);
        }

        // GET: Trades/Create
        public ActionResult Create()
        {
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "UserName");
            ViewBag.TradedToUserId = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Trades/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TradeId,QuantityOffered,TypeOffered,QuantityAsked,TypeAsked,Posted,IsActive,OwnerId,TradedToUserId")] Trade trade)
        {
            if (ModelState.IsValid)
            {
                db.Trades.Add(trade);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.OwnerId = new SelectList(db.Users, "Id", "UserName", trade.OwnerId);
            ViewBag.TradedToUserId = new SelectList(db.Users, "Id", "UserName", trade.TradedToUserId);
            return View(trade);
        }

        // GET: Trades/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trade trade = await db.Trades.FindAsync(id);
            if (trade == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "UserName", trade.OwnerId);
            ViewBag.TradedToUserId = new SelectList(db.Users, "Id", "UserName", trade.TradedToUserId);
            return View(trade);
        }

        // POST: Trades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "TradeId,QuantityOffered,TypeOffered,QuantityAsked,TypeAsked,Posted,IsActive,OwnerId,TradedToUserId")] Trade trade)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trade).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "UserName", trade.OwnerId);
            ViewBag.TradedToUserId = new SelectList(db.Users, "Id", "UserName", trade.TradedToUserId);
            return View(trade);
        }

        // GET: Trades/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trade trade = await db.Trades.FindAsync(id);
            if (trade == null)
            {
                return HttpNotFound();
            }
            return View(trade);
        }

        // POST: Trades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Trade trade = await db.Trades.FindAsync(id);
            db.Trades.Remove(trade);
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
