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
        // GET: Trades
        public async Task<ActionResult> Index()
        {
            var trades = this.Context.Trades.Include(t => t.Owner).Include(t => t.TradedToUser);
            return View(await trades.ToListAsync());
        }

        // GET: Trades/Create
        public ActionResult Create()
        {
            var vm = new Trade();
            vm.OwnerId = this.UserContext.Id;

            return View(vm);
        }

        // POST: Trades/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TradeId,QuantityOffered,TypeOffered,QuantityAsked,TypeAsked,Posted,OwnerId")] Trade trade)
        {
            if (ModelState.IsValid)
            {
                this.Context.Trades.Add(trade);
                await this.Context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(trade);
        }

        // GET: Trades/Accept/5
        public async Task<ActionResult> Accept(int? id)
        {
            throw new NotImplementedException();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trade trade = await this.Context.Trades.FindAsync(id);
            if (trade == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerId = new SelectList(this.Context.Users, "Id", "UserName", trade.OwnerId);
            ViewBag.TradedToUserId = new SelectList(this.Context.Users, "Id", "UserName", trade.TradedToUserId);
            return View(trade);
        }

        // GET: Trades/Cancel/5
        public async Task<ActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trade trade = await this.Context.Trades.FindAsync(id);
            if (trade == null)
            {
                return HttpNotFound();
            }
            return View(trade);
        }

        // POST: Trades/Cancel/5
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CancelConfirmed(int id)
        {
            Trade trade = await this.Context.Trades.FindAsync(id);
            trade.IsActive = false;
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
