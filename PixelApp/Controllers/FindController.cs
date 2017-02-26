using Pixel.Common.Data;
using PixelApp.Views.Find.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Controllers
{
    public class FindController : BaseController
    {
        protected Random rand = new Random();

        public ActionResult Zombies(bool hunt = false)
        {
            var vm = new FindZombiesViewModel();

            if (hunt)
            {
                if (this.UserContext.Energy < 10)
                {
                    vm.BattleText = "You don't have enough energy to hunt zombies. Wait for your energy to replenish!";
                }
                else if (this.UserContext.Life < 0)
                {
                    vm.BattleText = "You don't have any life. Zombies would surely kill you!";
                }
                else
                {
                    vm.ShowBattle = true;

                    // if win, +xp, +random res
                    // if lose, -xp, -random res
                    // if die, -xp, -more random res

                    // randomize win/lose for now
                    vm.IsWin = rand.Next(0, 10) < 8; // 80% chance win


                    var deltaLife = rand.Next(0, 10) + 5;

                    // todo: seriously, the combination of updating these fields needs to go somewhere consolidated
                    this.UserContext.Life -= deltaLife;
                    this.UserContext.LifeUpdatedTime = DateTime.Now;
                    if (this.UserContext.Life <= 0)
                    {
                        vm.IsWin = false;
                        vm.IsDead = true;
                        this.UserContext.Life = 0;
                    }

                    var deltaXp = vm.IsWin ? 10 : 5;
                    this.UserContext.Experience += deltaXp * (vm.IsWin ? 1 : vm.IsDead ? -2 : -1);

                    // use energy
                    // todo: seriously, the combination of updating these fields needs to go somewhere consolidated
                    this.UserContext.Energy -= 10; // todo: config
                    this.UserContext.EnergyUpdatedTime = DateTime.Now;

                    // setup battle text for user
                    vm.BattleText = $"You lost {deltaLife} life while hunting for zombies.";

                    vm.ResourceText = randomResource(vm.IsWin, vm.IsDead);

                    this.Context.SaveChanges();
                }
            }

            return View(vm);
        }

        public ActionResult Food(bool forage = false)
        {
            var vm = new FindFoodViewModel();

            if (forage)
            {
                vm.HasForaged = true;
                // todo: config
                var energyRequired = 10;

                if(this.UserContext.Energy < energyRequired)
                {
                    vm.Message = "You don't have enough energy to forage for food. Wait for your energy to replenish!";
                }
                else
                {
                    this.UserContext.Energy -= energyRequired;
                    this.UserContext.EnergyUpdatedTime = DateTime.Now;

                    // 60% chance of finding food
                    if(rand.Next(0, 10) < 6)
                    {
                        // 2-5 food for now X level
                        // todo: should this be some other f(level * factor) ?
                        var level = Services.StatManager.GetLevel(this.UserContext.Id, this.Context, false);
                        var qty = level * (rand.Next(0, 4) + 2);
                        vm.IsSuccess = true;
                        vm.Message = $"You spent {energyRequired} energy, and found {qty} food!";
                        this.UserContext.Food += qty;

                        // todo: config
                        this.UserContext.Experience += 2;

                        this.Context.SaveChanges();
                    }
                    else
                    {
                        // failed to find food
                        vm.Message = $"You spent {energyRequired} energy, and didn't find any food!";
                    }
                }
            }

            return View(vm);
        }

        /// <summary>
        /// Get random resource find/lose text based on non-zero resources
        /// </summary>
        /// <param name="isWin">Whether or not the player won the battle</param>
        /// <param name="isDie">Whether or not the player died during the fight</param>
        /// <returns>A formatted string indicating resource gfind/loss.</returns>
        private string randomResource(bool isWin, bool isDie)
        {
            var quantity = 0;

            // figure out what player has
            var list = new Dictionary<ResourceTypes, int>();
            if(this.UserContext.Water > 0)
            {
                list.Add(ResourceTypes.Water, this.UserContext.Water);
            }
            if (this.UserContext.Wood > 0)
            {
                list.Add(ResourceTypes.Wood, this.UserContext.Wood);
            }
            if (this.UserContext.Food > 0)
            {
                list.Add(ResourceTypes.Food, this.UserContext.Food);
            }
            if (this.UserContext.Stone > 0)
            {
                list.Add(ResourceTypes.Stone, this.UserContext.Stone);
            }
            if (this.UserContext.Oil > 0)
            {
                list.Add(ResourceTypes.Oil, this.UserContext.Oil);
            }
            if (this.UserContext.Iron > 0)
            {
                list.Add(ResourceTypes.Iron, this.UserContext.Iron);
            }

            // randomly find/lose a resource theyre carrying
            var resIndex = this.rand.Next(0, list.Count);
            var res = list.ElementAt(resIndex);

            // if win, normal gains, if lose, normal losses, if die, 2x losses
            if(res.Key == ResourceTypes.Water)
            {
                quantity = GetQuantity(this.UserContext.Water, isWin, isDie);
                this.UserContext.Water += quantity;
            }
            if (res.Key == ResourceTypes.Wood)
            {
                quantity = GetQuantity(this.UserContext.Wood, isWin, isDie);
                this.UserContext.Wood += quantity;
            }
            if (res.Key == ResourceTypes.Food)
            {
                quantity = GetQuantity(this.UserContext.Food, isWin, isDie);
                this.UserContext.Food += quantity;
            }
            if (res.Key == ResourceTypes.Stone)
            {
                quantity = GetQuantity(this.UserContext.Stone, isWin, isDie);
                this.UserContext.Stone += quantity;
            }
            if (res.Key == ResourceTypes.Oil)
            {
                quantity = GetQuantity(this.UserContext.Oil, isWin, isDie);
                this.UserContext.Oil += quantity;
            }
            if (res.Key == ResourceTypes.Iron)
            {
                quantity = GetQuantity(this.UserContext.Iron, isWin, isDie);
                this.UserContext.Iron += quantity;
            }

            return string.Format("You {0} {1} {2}.", isWin ? "found" : "lost", Math.Abs(quantity), res.Key);
        }

        private int GetQuantity(int userQty, bool isWin, bool isDie)
        {
            // todo: config the 0.02 percentage and multipliers for isWin/isDie/etc.
            var deltaQty = (int)(userQty * 0.02) * (isWin ? 1 : (isDie ? -2 : -1));

            // if they dont have enough to calculate 2%, gain/lose 1
            if(deltaQty == 0)
            {
                deltaQty = 1 * (isWin ? 1 : (isDie ? -2 : -1));
            }

            // don't set negative res values
            // yeah, this is a bit odd, but deltaQty should be indicative of gain/loss at this point
            if(userQty + deltaQty < 0)
            {
                deltaQty = 0;
            }

            return deltaQty;
        }
    }
}