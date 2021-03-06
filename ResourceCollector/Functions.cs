﻿using System;
using System.IO;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Pixel.Common.Cloud;
using System.Data.Entity;
using PixelApp.Models;
using Pixel.Common.Data;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using PixelApp.Services;

namespace ResourceCollector
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called addresources.
        public static void ProcessResourceMessage([QueueTrigger(QueueNames.ResourceQueue)] AddResourceMessage message)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.Include(x => x.Badges).Single(x => x.Id == message.UserId);

            var badges = db.Badges
                .Where(x => x.BadgeType == BadgeTypes.WaterCount 
                    || x.BadgeType == BadgeTypes.WoodCount
                    || x.BadgeType == BadgeTypes.FoodCount
                    || x.BadgeType == BadgeTypes.StoneCount
                    || x.BadgeType == BadgeTypes.OilCount
                    || x.BadgeType == BadgeTypes.IronCount
                )
                .ToList();

            var commit = false;

            foreach (var badge in badges.Where(x => x.BadgeType == BadgeTypes.WaterCount))
            {
                if (message.Water >= badge.Level && !user.Badges.Any(x => x.BadgeId == badge.BadgeId))
                {
                    AddBadgeToContext(db, user, badge);
                    commit = true;
                }
            }
            foreach (var badge in badges.Where(x => x.BadgeType == BadgeTypes.WoodCount))
            {
                if (message.Wood >= badge.Level && !user.Badges.Any(x => x.BadgeId == badge.BadgeId))
                {
                    AddBadgeToContext(db, user, badge);
                    commit = true;
                }
            }
            foreach (var badge in badges.Where(x => x.BadgeType == BadgeTypes.FoodCount))
            {
                if (message.Food >= badge.Level && !user.Badges.Any(x => x.BadgeId == badge.BadgeId))
                {
                    AddBadgeToContext(db, user, badge);
                    commit = true;
                }
            }
            foreach (var badge in badges.Where(x => x.BadgeType == BadgeTypes.StoneCount))
            {
                if (message.Stone >= badge.Level && !user.Badges.Any(x => x.BadgeId == badge.BadgeId))
                {
                    AddBadgeToContext(db, user, badge);
                    commit = true;
                }
            }
            foreach (var badge in badges.Where(x => x.BadgeType == BadgeTypes.OilCount))
            {
                if (message.Oil >= badge.Level && !user.Badges.Any(x => x.BadgeId == badge.BadgeId))
                {
                    AddBadgeToContext(db, user, badge);
                    commit = true;
                }
            }
            foreach (var badge in badges.Where(x => x.BadgeType == BadgeTypes.IronCount))
            {
                if (message.Iron >= badge.Level && !user.Badges.Any(x => x.BadgeId == badge.BadgeId))
                {
                    AddBadgeToContext(db, user, badge);
                    commit = true;
                }
            }

            if (commit)
            {
                db.SaveChanges();
            }
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

        public static void ProcessZombieFight([QueueTrigger(QueueNames.ZombieFights)] ZombieFightMessage message)
        {
            // add to azure table storage
            var table = GetGameEventTable();

            var model = new ZombieFightEntity
            {
                DeltaLife = message.DeltaLife,
                DeltaXp = message.DeltaXp,
                IsDead = message.IsDead,
                IsWin = message.IsWin,
                UserId = message.UserId,
            };

            var op = TableOperation.Insert(model);

            table.Execute(op);

            var zombieFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, TablePartitionKeys.GameEvents.ZombieFights);
            var userFilter = TableQuery.GenerateFilterCondition("UserId", QueryComparisons.Equal, message.UserId);
            var filter = TableQuery.CombineFilters(zombieFilter, TableOperators.And, userFilter);

            // check counts for badges
            var query = new TableQuery<ZombieFightEntity>().Where(filter);

            var fights = table.ExecuteQuery(query);

            var wins = fights.Count(x => x.IsWin.Equals(true));
            // todo: badge for losses
            //var losses = fights.Count(x => x.IsWin.Equals(false));

            AddBadges(wins, message.UserId, BadgeTypes.ZombieKills);
        }

        public static void ProcessFoodForage([QueueTrigger(QueueNames.FoodForage)] FoodForageMessage message)
        {
            // add to azure table storage
            var table = GetGameEventTable();

            var model = new FoodForageEntity
            {
                DeltaXp = message.DeltaXp,
                UserId = message.UserId,
                FoodFound = message.FoodFound,
            };

            var op = TableOperation.Insert(model);

            table.Execute(op);

            var foodFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, TablePartitionKeys.GameEvents.FoodForages);
            var userFilter = TableQuery.GenerateFilterCondition("UserId", QueryComparisons.Equal, message.UserId);
            var filter = TableQuery.CombineFilters(foodFilter, TableOperators.And, userFilter);

            // check counts for badges
            var query = new TableQuery<FoodForageEntity>().Where(filter);

            var forageCount = table.ExecuteQuery(query).Count();

            AddBadges(forageCount, message.UserId, BadgeTypes.FoodForages);
        }

        public static void ProcessResearchCompleted([QueueTrigger(QueueNames.ResearchCompleted)] ResearchCompletedMessage message)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.Include(x => x.Badges).Single(x => x.Id == message.UserId);
            var researchCount = db.UserTechnologies.Count(x => x.UserId == message.UserId && x.StatusId == UserTechnologyStatusTypes.Researched);
            var badges = db.Badges.Where(x => x.BadgeType == BadgeTypes.ResearchedSubjects);
            var commit = false;

            foreach (var badge in badges)
            {
                if(researchCount >= badge.Level && !user.Badges.Any(x => x.BadgeId == badge.BadgeId))
                {
                    AddBadgeToContext(db, user, badge);
                    commit = true;
                }
            }

            if (commit)
            {
                db.SaveChanges();
            }
        }

        private static void AddBadges(int level, string userId, BadgeTypes type)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.Include(x => x.Badges).Single(x => x.Id == userId);
            var badges = db.Badges.Where(x => x.BadgeType == type);
            var commit = false;

            foreach (var badge in badges)
            {
                // if user doesn't have badge and they've met the level e.g. 10 zombie kills, give them the badge
                if (level >= badge.Level && !user.Badges.Any(x => x.BadgeId == badge.BadgeId))
                {
                    AddBadgeToContext(db, user, badge);

                    commit = true;
                }
            }

            if (commit)
            {
                db.SaveChanges();
            }
        }

        private static void AddBadgeToContext(ApplicationDbContext db, ApplicationUser user, Badge badge)
        {
            user.Experience += badge.ExperienceGain;

            // todo: create badge service or something similar for this operation
            // create user badge
            var newBadge = new UserBadge
            {
                BadgeId = badge.BadgeId,
                UserId = user.Id,
                Created = DateTime.Now,
            };
            db.UserBadges.Add(newBadge);

            // create notification
            var note = CommunicationService.CreateNotification(
                user.Id,
                "You earned a badge!",
                $"You've been conferred a new badge for: {badge.Name}. You earned {badge.ExperienceGain} experience!!");

            db.Notes.Add(note);
        }

        private static CloudTable GetGameEventTable()
        {
            var storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager
                            .ConnectionStrings["StorageConnectionString"].ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableNames.GameEvents);
            table.CreateIfNotExists();

            return table;
        }
    }
}
