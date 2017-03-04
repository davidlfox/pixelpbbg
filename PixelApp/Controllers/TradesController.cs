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
                switch (trade.TypeOffered)
                {
                    case Pixel.Common.Data.ResourceTypes.Water:
                        if (this.UserContext.Water < trade.QuantityOffered)
                        {
                            ModelState.AddModelError(string.Empty, quantityError);
                        }
                        break;
                    case Pixel.Common.Data.ResourceTypes.Wood:
                        if (this.UserContext.Wood < trade.QuantityOffered)
                        {
                            ModelState.AddModelError(string.Empty, quantityError);
                        }
                        break;
                    case Pixel.Common.Data.ResourceTypes.Food:
                        if (this.UserContext.Food < trade.QuantityOffered)
                        {
                            ModelState.AddModelError(string.Empty, quantityError);
                        }
                        break;
                    case Pixel.Common.Data.ResourceTypes.Stone:
                        if (this.UserContext.Stone < trade.QuantityOffered)
                        {
                            ModelState.AddModelError(string.Empty, quantityError);
                        }
                        break;
                    case Pixel.Common.Data.ResourceTypes.Oil:
                        if (this.UserContext.Oil < trade.QuantityOffered)
                        {
                            ModelState.AddModelError(string.Empty, quantityError);
                        }
                        break;
                    case Pixel.Common.Data.ResourceTypes.Iron:
                        if (this.UserContext.Iron < trade.QuantityOffered)
                        {
                            ModelState.AddModelError(string.Empty, quantityError);
                        }
                        break;
                    default:
                        ModelState.AddModelError(string.Empty, "The trade offer must include a type of resource");
                        break;
                }

                if (ModelState.IsValid)
                {
                    // reduce resources by offer amount
                    switch (trade.TypeOffered)
                    {
                        case Pixel.Common.Data.ResourceTypes.Water:
                            this.UserContext.Water -= trade.QuantityOffered;
                            break;
                        case Pixel.Common.Data.ResourceTypes.Wood:
                            this.UserContext.Wood -= trade.QuantityOffered;
                            break;
                        case Pixel.Common.Data.ResourceTypes.Food:
                            this.UserContext.Food -= trade.QuantityOffered;
                            break;
                        case Pixel.Common.Data.ResourceTypes.Stone:
                            this.UserContext.Stone -= trade.QuantityOffered;
                            break;
                        case Pixel.Common.Data.ResourceTypes.Oil:
                            this.UserContext.Oil -= trade.QuantityOffered;
                            break;
                        case Pixel.Common.Data.ResourceTypes.Iron:
                            this.UserContext.Iron -= trade.QuantityOffered;
                            break;
                    }

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
            switch (trade.TypeAsked)
            {
                case Pixel.Common.Data.ResourceTypes.Water:
                    if (this.UserContext.Water < trade.QuantityAsked)
                    {
                        return RedirectToAction("Index", new { iserror = true, message = tradeErrorMessage });
                    }
                    else
                    {
                        trade.IsActive = false;
                        // take away ask from accepting trader
                        this.UserContext.Water -= trade.QuantityAsked;
                        // give ask to initiating trader
                        trade.Owner.Water += trade.QuantityAsked;
                    }
                    break;
                case Pixel.Common.Data.ResourceTypes.Wood:
                    if (this.UserContext.Wood < trade.QuantityAsked)
                    {
                        return RedirectToAction("Index", new { iserror = true, message = tradeErrorMessage });
                    }
                    else
                    {
                        trade.IsActive = false;
                        this.UserContext.Wood -= trade.QuantityAsked;
                        trade.Owner.Wood += trade.QuantityAsked;
                    }
                    break;
                case Pixel.Common.Data.ResourceTypes.Food:
                    if (this.UserContext.Food < trade.QuantityAsked)
                    {
                        return RedirectToAction("Index", new { iserror = true, message = tradeErrorMessage });
                    }
                    else
                    {
                        trade.IsActive = false;
                        this.UserContext.Food -= trade.QuantityAsked;
                        trade.Owner.Food += trade.QuantityAsked;
                    }
                    break;
                case Pixel.Common.Data.ResourceTypes.Stone:
                    if (this.UserContext.Stone < trade.QuantityAsked)
                    {
                        return RedirectToAction("Index", new { iserror = true, message = tradeErrorMessage });
                    }
                    else
                    {
                        trade.IsActive = false;
                        this.UserContext.Stone -= trade.QuantityAsked;
                        trade.Owner.Stone += trade.QuantityAsked;
                    }
                    break;
                case Pixel.Common.Data.ResourceTypes.Oil:
                    if (this.UserContext.Oil < trade.QuantityAsked)
                    {
                        return RedirectToAction("Index", new { iserror = true, message = tradeErrorMessage });
                    }
                    else
                    {
                        trade.IsActive = false;
                        this.UserContext.Oil -= trade.QuantityAsked;
                        trade.Owner.Oil += trade.QuantityAsked;
                    }
                    break;
                case Pixel.Common.Data.ResourceTypes.Iron:
                    if (this.UserContext.Iron < trade.QuantityAsked)
                    {
                        return RedirectToAction("Index", new { iserror = true, message = tradeErrorMessage });
                    }
                    else
                    {
                        trade.IsActive = false;
                        this.UserContext.Iron -= trade.QuantityAsked;
                        trade.Owner.Iron += trade.QuantityAsked;
                    }
                    break;
                default:
                    break;
            }

            // at this point, the ask has been satisfied. give offer quantity to this user
            switch (trade.TypeOffered)
            {
                case Pixel.Common.Data.ResourceTypes.Water:
                    this.UserContext.Water += trade.QuantityOffered;
                    break;
                case Pixel.Common.Data.ResourceTypes.Wood:
                    this.UserContext.Wood += trade.QuantityOffered;
                    break;
                case Pixel.Common.Data.ResourceTypes.Food:
                    this.UserContext.Food += trade.QuantityOffered;
                    break;
                case Pixel.Common.Data.ResourceTypes.Stone:
                    this.UserContext.Stone += trade.QuantityOffered;
                    break;
                case Pixel.Common.Data.ResourceTypes.Oil:
                    this.UserContext.Oil += trade.QuantityOffered;
                    break;
                case Pixel.Common.Data.ResourceTypes.Iron:
                    this.UserContext.Iron += trade.QuantityOffered;
                    break;
                default:
                    break;
            }

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
            switch (trade.TypeOffered)
            {
                case Pixel.Common.Data.ResourceTypes.Water:
                    this.UserContext.Water += trade.QuantityOffered;
                    break;
                case Pixel.Common.Data.ResourceTypes.Wood:
                    this.UserContext.Wood += trade.QuantityOffered;
                    break;
                case Pixel.Common.Data.ResourceTypes.Food:
                    this.UserContext.Food += trade.QuantityOffered;
                    break;
                case Pixel.Common.Data.ResourceTypes.Stone:
                    this.UserContext.Stone += trade.QuantityOffered;
                    break;
                case Pixel.Common.Data.ResourceTypes.Oil:
                    this.UserContext.Oil += trade.QuantityOffered;
                    break;
                case Pixel.Common.Data.ResourceTypes.Iron:
                    this.UserContext.Iron += trade.QuantityOffered;
                    break;
                default:
                    break;
            }

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
