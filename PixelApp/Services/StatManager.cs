using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Services
{
    public class StatManager
    {
        public static readonly int BaseExperience = 200;
        public static readonly int ExperienceFactor = 2;

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
                var elapsedMinutes = (int)elapsed.TotalMinutes;

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

        // todo: this is basically the same function as GetEnergy--figure out how to do this with Func/lambdas to make it more generic?

        /// <summary>
        /// gets a user's life stat with an option to commit updates (based on time, for example) to the db
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="db">db context</param>
        /// <param name="commit">whether or not to commit this single change to the db</param>
        /// <returns>the user's up-to-date life</returns>
        public static int GetLife(string userId, ApplicationDbContext db, bool commit = true)
        {
            var user = db.Users.Single(x => x.Id == userId);

            if (user.LifeUpdatedTime.HasValue)
            {
                // add life per time interval
                var elapsed = DateTime.Now - user.LifeUpdatedTime.Value;
                var elapsedMinutes = (int)elapsed.TotalMinutes;

                if (elapsedMinutes >= 1) // todo: config this
                {
                    var newLife = user.Life + elapsedMinutes;
                    if (newLife >= user.MaxLife)
                    {
                        user.Life = user.MaxLife;
                        user.LifeUpdatedTime = null;
                    }
                    else
                    {
                        user.Life = newLife;

                        // reset life update time to most recent minute interval, so calculations keep working
                        // e.g. if 2:34 has elapsed, reset time to 0:34 ago
                        var diff = TimeSpan.FromTicks(elapsed.Ticks % TimeSpan.FromSeconds(60).Ticks);
                        user.LifeUpdatedTime = DateTime.Now - diff;
                    }
                }

                if (commit)
                {
                    db.SaveChanges();
                }
            }

            return user.Life;
        }

        /// <summary>
        /// get player level accounting for any recent experience
        /// probably something like: 300x - 150
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="db">db context</param>
        /// <param name="commit">whether or not to commit this single change to the db</param>
        /// <returns>player level after updating for experience</returns>
        public static int GetLevel(string userId, ApplicationDbContext db, bool commit = true)
        {
            var user = db.Users.Single(x => x.Id == userId);

            var experienceNeeded = BaseExperience * Math.Pow(user.Level, ExperienceFactor);

            if(user.Experience >= experienceNeeded)
            {
                user.Level++;

                if (commit)
                {
                    db.SaveChanges();
                }
            }

            return user.Level;
        }

        /// <summary>
        /// Get progress to next level as a percentage
        /// </summary>
        /// <param name="level">current user level</param>
        /// <param name="experience">current user experience</param>
        /// <returns>Progress as a percentage</returns>
        public static double GetLevelProgress(int level, int experience)
        {
            double lastLevelExperience = 0;

            if (level > 1)
            {
                lastLevelExperience = BaseExperience * Math.Pow(level - 1, ExperienceFactor);
            }

            var experienceNeeded = BaseExperience * Math.Pow(level, ExperienceFactor);

            var percentage = (experience - lastLevelExperience) / (experienceNeeded - lastLevelExperience);

            // e.g. player lost a bunch of experience shortly after gaining a level, but it doesn't revert them a level
            if (percentage < 0)
            {
                percentage = 0;
            }

            return percentage;
        }
    }
}