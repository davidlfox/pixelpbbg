using Pixel.Common.Cloud;
using Pixel.Common.Data;
using PixelApp.Models;
using PixelApp.Services;
using PixelApp.Views.Find.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Controllers
{
    [Authorize]
    public class FindController : BaseController
    {
        protected Random rand = new Random();

        public async Task<ActionResult> Zombies(bool hunt = false)
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

                    // Calculate winPercentage
                    var offenseBoosts = this.Context.UserTechnologies.Include("Technology").Where(x => x.UserId.Equals(this.UserContext.Id)
                        && x.StatusId == UserTechnologyStatusTypes.Researched
                        && x.Technology.BoostTypeId == BoostTypes.Offense)
                        .Select(x => x.Technology.BoostAmount).ToList();
                    var winPercentage = 80 + (offenseBoosts.Sum() * 100);

                    // randomize win/lose for now
                    vm.IsWin = rand.Next(0, 100) < winPercentage; // 80% + boosts chance win

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

                    var qm = new QueueManager();
                    await qm.QueueZombieFight(this.UserContext.Id, deltaXp, deltaLife, vm.IsWin, vm.IsDead);
                }
            }

            return View(vm);
        }

        public async Task<ActionResult> Food(bool forage = false)
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

                        var level = StatManager.GetLevel(this.UserContext.Id, this.Context);

                        var qty = level * (rand.Next(0, 4) + 4);
                        vm.IsSuccess = true;
                        vm.Message = $"You spent {energyRequired} energy, and found {qty} food!";
                        var food = this.UserContext.Items.Single(x => x.ItemId == (int)ResourceTypes.Food);
                        food.Quantity += qty;

                        // todo: config
                        this.UserContext.Experience += 2;

                        this.Context.SaveChanges();

                        var qm = new QueueManager();
                        await qm.QueueFoodForage(this.UserContext.Id, 2, qty);
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

        public ActionResult People(bool find = false)
        {
            var vm = new FindPeopleViewModel();

            if (find)
            {
                vm.HasRecruited = true;
                // todo: config
                var energyRequired = 50;

                var userEnergy = StatManager.GetEnergy(this.UserContext.Id, this.Context);

                if (userEnergy < energyRequired)
                {
                    vm.Message = "You don't have enough energy to recruit people. Wait for your energy to replenish!";
                }
                else
                {
                    this.UserContext.Energy -= energyRequired;
                    this.UserContext.EnergyUpdatedTime = DateTime.Now;

                    var cultureTechs = this.Context.UserTechnologies
                        .Include("Technology")
                        .Where(x => x.UserId.Equals(this.UserContext.Id)
                            && x.StatusId == UserTechnologyStatusTypes.Researched
                            && x.Technology.BoostTypeId == BoostTypes.Population);

                    var cultureBoost = 0.0m;

                    if (cultureTechs.Any())
                    {
                        cultureBoost = cultureTechs.Sum(x => x.Technology.BoostAmount);
                    }

                    // find random people between (level and level X 4) + culture boosts
                    var level = StatManager.GetLevel(this.UserContext.Id, this.Context);
                    var upperBound = level * 4;
                    var qty = (int)(rand.Next(level, upperBound) * (1m + cultureBoost));

                    vm.IsSuccess = true;
                    vm.Message = $"You venture into the outskirts, spending {energyRequired} energy, and you recruit {qty} people!";

                    this.UserContext.Territory.CivilianPopulation += qty;

                    // todo: config
                    this.UserContext.Experience += 5;

                    this.Context.SaveChanges();
                }
            }

            return View(vm);
        }
    }
}