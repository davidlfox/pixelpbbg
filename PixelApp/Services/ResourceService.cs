using Pixel.Common.Data;
using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Services
{
    public class ResourceService
    {
        /// <summary>
        /// Get random resource find/lose text based on non-zero resources
        /// </summary>
        /// <param name="isWin">Whether or not the player won the battle</param>
        /// <param name="isDie">Whether or not the player died during the fight</param>
        /// <returns>A formatted string indicating resource gfind/loss.</returns>
        public static string RandomResource(ApplicationUser user, bool isWin, bool isDie, double percentageLoss)
        {
            var quantity = 0;
            var rand = new Random();

            // figure out what player has
            Dictionary<ResourceTypes, int> list = GetNonZeroUserResources(user);

            // randomly find/lose a resource theyre carrying
            var resIndex = rand.Next(0, list.Count);
            var res = list.ElementAt(resIndex);

            // if win, normal gains, if lose, normal losses, if die, 2x losses
            if (res.Key == ResourceTypes.Water)
            {
                quantity = GetQuantity(user.Water, isWin, isDie, percentageLoss);
                user.Water += quantity;
            }
            if (res.Key == ResourceTypes.Wood)
            {
                quantity = GetQuantity(user.Wood, isWin, isDie, percentageLoss);
                user.Wood += quantity;
            }
            if (res.Key == ResourceTypes.Food)
            {
                quantity = GetQuantity(user.Food, isWin, isDie, percentageLoss);
                user.Food += quantity;
            }
            if (res.Key == ResourceTypes.Stone)
            {
                quantity = GetQuantity(user.Stone, isWin, isDie, percentageLoss);
                user.Stone += quantity;
            }
            if (res.Key == ResourceTypes.Oil)
            {
                quantity = GetQuantity(user.Oil, isWin, isDie, percentageLoss);
                user.Oil += quantity;
            }
            if (res.Key == ResourceTypes.Iron)
            {
                quantity = GetQuantity(user.Iron, isWin, isDie, percentageLoss);
                user.Iron += quantity;
            }

            return string.Format("You {0} {1} {2}.", isWin ? "found" : "lost", Math.Abs(quantity), res.Key);
        }

        public static Dictionary<ResourceTypes, int> GetNonZeroUserResources(ApplicationUser user)
        {
            var list = new Dictionary<ResourceTypes, int>();
            if (user.Water > 0)
            {
                list.Add(ResourceTypes.Water, user.Water);
            }
            if (user.Wood > 0)
            {
                list.Add(ResourceTypes.Wood, user.Wood);
            }
            if (user.Food > 0)
            {
                list.Add(ResourceTypes.Food, user.Food);
            }
            if (user.Stone > 0)
            {
                list.Add(ResourceTypes.Stone, user.Stone);
            }
            if (user.Oil > 0)
            {
                list.Add(ResourceTypes.Oil, user.Oil);
            }
            if (user.Iron > 0)
            {
                list.Add(ResourceTypes.Iron, user.Iron);
            }

            return list;
        }

        public static int GetQuantity(int userQty, bool isWin, bool isDie, double percent)
        {
            // todo: config the 0.02 percentage and multipliers for isWin/isDie/etc.
            var deltaQty = (int)(userQty * percent) * (isWin ? 1 : (isDie ? -2 : -1));

            // if they dont have enough to calculate 2%, gain/lose 1
            if (deltaQty == 0)
            {
                deltaQty = 1 * (isWin ? 1 : (isDie ? -2 : -1));
            }

            // don't set negative res values
            // yeah, this is a bit odd, but deltaQty should be indicative of gain/loss at this point
            if (userQty + deltaQty < 0)
            {
                deltaQty = 0;
            }

            return deltaQty;
        }
    }
}