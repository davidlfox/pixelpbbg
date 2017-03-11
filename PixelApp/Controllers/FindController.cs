using Pixel.Common.Data;
using PixelApp.Services;
using PixelApp.Views.Find.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Controllers
{
    [Authorize]
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

                    // todo: config
                    var percentageLoss = 0.02;
                    vm.ResourceText = ResourceService.RandomResource(this.UserContext, vm.IsWin, vm.IsDead, percentageLoss);

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

                        // this is intentionally using the .Level property to avoid race condition when messaging about leveling up
                        var level = StatManager.GetLevel(this.UserContext.Id, this.Context);

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
    }
}