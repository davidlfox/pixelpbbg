using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Services
{
    public class StatManager
    {
        /// <summary>
        /// gets a user's energy stat with an option to commit updates (based on time, for example) to the db
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="db">db context</param>
        /// <param name="commit">whether or not to commit this single change to the db</param>
        /// <returns>the user's up-to-date energy</returns>
        public static int GetEnergy(string userId, ApplicationDbContext db, bool commit = true)
        {
            var user = db.Users.Single(x => x.Id == userId);

            if (user.EnergyUpdatedTime.HasValue)
            {
                // add energy per time interval
                var elapsed = DateTime.Now - user.EnergyUpdatedTime.Value;
                var elapsedMinutes = elapsed.Minutes;

                if (elapsedMinutes >= 1) // todo: config this
                {
                    var newEnergy = user.Energy + elapsedMinutes;
                    if (newEnergy >= user.MaxEnergy)
                    {
                        user.Energy = user.MaxEnergy;
                        user.EnergyUpdatedTime = null;
                    }
                    else
                    {
                        user.Energy = newEnergy;

                        // reset energy update time to most recent minute interval, so calculations keep working
                        // e.g. if 2:34 has elapsed, reset time to 0:34 ago
                        var diff = TimeSpan.FromTicks(elapsed.Ticks % TimeSpan.FromSeconds(60).Ticks);
                        user.EnergyUpdatedTime = DateTime.Now - diff;
                    }
                }

                if (commit)
                {
                    db.SaveChanges();
                }
            }

            return user.Energy;
        }
    }
}