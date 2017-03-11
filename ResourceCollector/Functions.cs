using System;
using System.IO;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Pixel.Common.Cloud;
using System.Data.Entity;
using PixelApp.Models;
using PixelApp.Services;
using Pixel.Common.Data;

namespace ResourceCollector
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called addresources.
        public static void ProcessResourceMessage([QueueTrigger(QueueNames.ResourceQueue)] AddResourceMessage message, TextWriter log)
        {
            // tbd
        }

        public static void ProcessPopulationMessage([QueueTrigger(QueueNames.PopulationQueue)] AddPopulationMessage message)
        {
            // tbd
        }

        public static void ProcessNightlyAttack([QueueTrigger(QueueNames.NightlyAttackQueue)] NightlyAttackMessage message)
        {
            // tbd
        }

        public static void ProcessExperience([QueueTrigger(QueueNames.Experience)] ExperienceMessage message)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Single(x => x.Id == message.UserId);
                user.Experience += message.Experience;
                db.SaveChanges();
            }
        }
    }
}
