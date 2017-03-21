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
                quantity = GetQuantity(res.Value, isWin, isDie, 0);
                var item = user.Items.Single(x => x.ItemId == (int)res.Key);
                item.Quantity += quantity;

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

            // probably a .ToDictionary way to do this
            return user.Items.Where(x => x.Item.IsCore.Equals(true) && x.Quantity > 0)
                .ToDictionary(x => (ResourceTypes)x.ItemId, x => x.Quantity);
            
            //foreach (var item in items)
            //{
            //    list.Add((ResourceTypes)item.ItemId, item.Quantity);
            //}
                
            //return list;
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