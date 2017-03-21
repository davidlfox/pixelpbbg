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
using PixelApp.Services;

namespace PixelApp.Controllers
{
    [Authorize]
    public class TradesController : BaseController
    {
        // GET: Trades
        public async Task<ActionResult> Index(bool isError = false, string message = "")
        {
            ViewBag.IsError = isError;
            ViewBag.Message = message;

            IQueryable<Trade> trades = GetTrades();
            return View(await trades.ToListAsync());
        }

        private IQueryable<Trade> GetTrades()
        {
            return this.Context.Trades
                .Include(t => t.Owner)
                .Include(t => t.TradedToUser)
                .Where(t => t.IsActive.Equals(true));
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
        public async Task<ActionResult> Create([Bind(Include = "TradeId,QuantityOffered,TypeOffered,QuantityAsked,TypeAsked,OwnerId")] Trade trade)
        {
            trade.Posted = DateTime.Now;
            trade.IsActive = true;

            if (ModelState.IsValid)
            {
                var quantityError = "You don't have that much to offer";
                // validate user has the offered resources
                // todo: this sucks, having to explicitly check different attributes. should resources be a collection on the user??
                var itemOffered = this.UserContext.Items.Single(x => x.ItemId == (int)trade.TypeOffered);
                if (itemOffered.Quantity < trade.QuantityOffered)
                {
                    ModelState.AddModelError(string.Empty, quantityError);
                }

                if (ModelState.IsValid)
                {
                    // reduce resources by offer amount
                    itemOffered.Quantity -= trade.QuantityOffered;

                    this.Context.Trades.Add(trade);
                    await this.Context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            return View(trade);
        }

        // GET: Trades/Accept/5
        public async Task<ActionResult> Accept(int? id)
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

            // going to return this anyway
            if (trade.OwnerId == this.UserContext.Id)
            {
                return RedirectToAction("Index", new { iserror = true, message = "You can't accept your own trade." });
            }

            var tradeErrorMessage = "You don't have enough to accept that trade";

            // validate accepting user has resources, then take them away, give them to the initiating trader
            // todo: notify initiating user that trade has been accepted

            var itemAsked = this.UserContext.Items.Single(x => x.ItemId == (int)trade.TypeAsked);
            if (itemAsked.Quantity < trade.QuantityAsked)
            {
                return RedirectToAction("Index", new { iserror = true, message = tradeErrorMessage });
            }
            else
            {
                trade.IsActive = false;
                // take away ask from accepting trader
                itemAsked.Quantity -= trade.QuantityAsked;
                // give ask to initiating trader
                var tradeOwner = this.Context.Users
                    .Include(x => x.Items)
                    .Single(x => x.Id == trade.OwnerId);
                var tradeOwnerItem = tradeOwner.Items.Single(x => x.ItemId == (int)trade.TypeAsked);
                tradeOwnerItem.Quantity += trade.QuantityAsked;
            }

            // at this point, the ask has been satisfied. give offer quantity to this user
            var acceptingUserItem = this.UserContext.Items.Single(x => x.ItemId == (int)trade.TypeOffered);
            acceptingUserItem.Quantity += trade.QuantityOffered;

            trade.TradedToUserId = this.UserContext.Id;

            // notify initiating user of completed trade
            // todo: queue this
            var note = CommunicationService.CreateNotification(
                trade.OwnerId,
                $"Your trade was accepted by {this.UserContext.UserName}!",
                $"{this.UserContext.UserName} traded you {trade.QuantityAsked} {trade.TypeAsked} " +
                    $"for {trade.QuantityOffered} of your {trade.TypeOffered}.");

            this.Context.Notes.Add(note);

            return RedirectToAction("Index", new { message = "You successfully completed the trade." });
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
            if (trade.OwnerId != this.UserContext.Id)
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
            if (trade.OwnerId != this.UserContext.Id)
            {
                return HttpNotFound();
            }
            trade.IsActive = false;

            // give resources back to the user
            var item = this.UserContext.Items.Single(x => x.ItemId == (int)trade.TypeOffered);
            item.Quantity += trade.QuantityOffered;

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
