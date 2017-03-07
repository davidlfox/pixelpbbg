using Pixel.Common.Models;
using PixelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
            var user = context.Users.Where(x => x.Id.Equals(userId)).FirstOrDefault();
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

            switch (tech.ResourceCostTypeId)
            {
                case Pixel.Common.Data.ResourceTypes.Water:
                    if (user.Water < tech.ResourceCost)
                        return new ProcessResponse(false, string.Format("You need {0} more Water to begin researching {1}", tech.ResourceCost - user.Water, tech.Name));
                    else
                        user.Water = user.Water - tech.ResourceCost;
                        break;
                case Pixel.Common.Data.ResourceTypes.Wood:
                    if (user.Wood < tech.ResourceCost)
                        return new ProcessResponse(false, string.Format("You need {0} more Wood to begin researching {1}", tech.ResourceCost - user.Wood, tech.Name));
                    else
                        user.Wood = user.Wood - tech.ResourceCost;
                    break;
                case Pixel.Common.Data.ResourceTypes.Food:
                    if (user.Food < tech.ResourceCost)
                        return new ProcessResponse(false, string.Format("You need {0} more Food to begin researching {1}", tech.ResourceCost - user.Food, tech.Name));
                    else
                        user.Food = user.Food - tech.ResourceCost;
                    break;
                case Pixel.Common.Data.ResourceTypes.Stone:
                    if (user.Stone < tech.ResourceCost)
                        return new ProcessResponse(false, string.Format("You need {0} more Stone to begin researching {1}", tech.ResourceCost - user.Stone, tech.Name));
                    else
                        user.Stone = user.Stone - tech.ResourceCost;
                    break;
                case Pixel.Common.Data.ResourceTypes.Oil:
                    if (user.Oil < tech.ResourceCost)
                        return new ProcessResponse(false, string.Format("You need {0} more Oil to begin researching {1}", tech.ResourceCost - user.Oil, tech.Name));
                    else
                        user.Oil = user.Oil - tech.ResourceCost;
                    break;
                case Pixel.Common.Data.ResourceTypes.Iron:
                    if (user.Iron < tech.ResourceCost)
                        return new ProcessResponse(false, string.Format("You need {0} more Iron to begin researching {1}", tech.ResourceCost - user.Iron, tech.Name));
                    else
                        user.Iron = user.Iron - tech.ResourceCost;
                    break;
                default:
                    break;
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