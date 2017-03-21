using Pixel.Common.Cloud;
using Pixel.Common.Data;
using Pixel.Common.Models;
using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace PixelApp.Services
{
    public class TechnologyService
    {
        private ApplicationDbContext context;

        public TechnologyService()
        {
            this.context = new ApplicationDbContext();
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        public IQueryable<Technology> GetTechnologies()
        {
            return context.Technologies;
        }

        public IQueryable<UserTechnology> GetUserTechnologies()
        {
            return context.UserTechnologies;
        }

        public ProcessResponse StartResearch(int technologyId, string userId)
        {
            // Get records needed for starting research
            var tech = GetById(technologyId);
            var user = context.Users
                .Include(x => x.Items)
                .FirstOrDefault(x => x.Id.Equals(userId));

            if (tech == null || user == null)
                return new ProcessResponse(false, "There was an error starting this research, please try again later.");

            // Esure that required resources, energy and prerequisites are available
            if (user.Energy < tech.EnergyCost)
                return new ProcessResponse(false, string.Format("You need {0} more energy to begin researching {1}", tech.EnergyCost - user.Energy, tech.Name));

            if (tech.PreRequisiteId != null)
            {
                if (context.UserTechnologies.Where(x =>
                        x.UserTechnologyId.Equals(tech.PreRequisiteId)
                        && x.UserId.Equals(userId)
                        && x.StatusId == UserTechnologyStatusTypes.Researched
                        ).Any())
                    return new ProcessResponse(false, string.Format("Researching {0} requires that you first research", tech.Name, tech.PreRequisite.Name));
            }

            var item = user.Items.Single(x => x.ItemId == (int)tech.ResourceCostTypeId);
            if (item.Quantity < tech.ResourceCost)
            {
                var diff = tech.ResourceCost - item.Quantity;
                return new ProcessResponse(false, $"You need {diff} more {item.Item.Name} to begin researching {tech.Name}");
            }
            else
            {
                item.Quantity -= tech.ResourceCost;
            }

            // update the user record accordingly
            user.Energy = user.Energy - tech.EnergyCost;

            // Add new usertechnology record
            context.UserTechnologies.Add(new UserTechnology
            {
                TechnologyId = technologyId,
                UserId = userId,
                ResearchStartDate = DateTime.Now,
                StatusId = UserTechnologyStatusTypes.Pending,
            });

            return new ProcessResponse(true, string.Format("Research has begun on {0}", tech.Name));
        }

        public UserTechnology GetCheckPendingResearch(string userId)
        {
            var pending = context.UserTechnologies.Include("Technology")
                .FirstOrDefault(x => x.UserId.Equals(userId) && x.StatusId == UserTechnologyStatusTypes.Pending);

            if (pending != null)
            {
                if ((DateTime.Now - pending.ResearchStartDate).TotalDays > pending.Technology.ResearchDays)
                {
                    pending.StatusId = UserTechnologyStatusTypes.Researched;

                    var tech = pending.Technology;
                    // notify user
                    var note = CommunicationService.CreateNotification(
                        userId,
                        "Your research is complete!",
                        $"You finished researching {tech.Name} and gained {tech.BoostAmount:P1} {tech.BoostTypeId}");

                    context.Notes.Add(note);

                    var qm = new QueueManager();
                    qm.QueueResearchCompleted(userId);

                    return null;
                }
            }

            return pending;
        }

        public Technology GetById(int technologyId)
        {
            return context.Technologies.Find(technologyId);
        }

        public List<int> GetResearchedTechnologyIds(string userId)
        {
            return context.UserTechnologies
                .Where(x => x.UserId.Equals(userId) && x.StatusId == UserTechnologyStatusTypes.Researched)
                .Select(x => x.TechnologyId)
                .ToList();
        }
    }
}