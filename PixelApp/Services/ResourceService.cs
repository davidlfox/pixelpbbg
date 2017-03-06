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
        static Random rand = new Random();

        /// <summary>
        /// Get random resource find/lose text based on non-zero resources
        /// </summary>
        /// <param name="isWin">Whether or not the player won the battle</param>
        /// <param name="isDie">Whether or not the player died during the fight</param>
        /// <returns>A formatted string indicating resource gfind/loss.</returns>
        public static string RandomResource(ApplicationUser user, bool isWin, bool isDie, double percentageLoss)
        {
            var quantity = 0;

            // figure out what player has
            Dictionary<ResourceTypes, int> list = GetNonZeroUserResources(user);

            if (list.Count > 0)
            {
                // randomly find/lose a resource theyre carrying
                var resIndex = rand.Next(0, list.Count);
                var res = list.ElementAt(resIndex);

                // if win, normal gains, if lose, normal losses, if die, 2x losses
                if (res.Key == ResourceTypes.Water)
                {
                    quantity = GetQuantity(user.Water, isWin, isDie, 0);
                    user.Water += quantity;
                }
                if (res.Key == ResourceTypes.Wood)
                {
                    quantity = GetQuantity(user.Wood, isWin, isDie, 0);
                    user.Wood += quantity;
                }
                if (res.Key == ResourceTypes.Food)
                {
                    quantity = GetQuantity(user.Food, isWin, isDie, 0);
                    user.Food += quantity;
                }
                if (res.Key == ResourceTypes.Stone)
                {
                    quantity = GetQuantity(user.Stone, isWin, isDie, 0);
                    user.Stone += quantity;
                }
                if (res.Key == ResourceTypes.Oil)
                {
                    quantity = GetQuantity(user.Oil, isWin, isDie, 0);
                    user.Oil += quantity;
                }
                if (res.Key == ResourceTypes.Iron)
                {
                    quantity = GetQuantity(user.Iron, isWin, isDie, 0);
                    user.Iron += quantity;
                }

                return string.Format("You {0} {1} {2}.", isWin ? "found" : "lost", Math.Abs(quantity), res.Key);
            }
            else
            {
                return string.Format("You {0} no resources.", isWin ? "found" : "lost");
            }

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

        public static int GetQuantity(int userQty, bool isWin, bool isDie, double deltaPercentage)
        {
            var baseDelta = deltaPercentage > 0 ? (int)(userQty * deltaPercentage) : rand.Next(0, 15) + 5;

            // todo: config the 0.02 percentage and multipliers for isWin/isDie/etc.
            var deltaQty = baseDelta * (isWin ? 1 : (isDie ? -2 : -1));

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