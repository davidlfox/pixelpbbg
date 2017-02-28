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

        public void StartResearch(int technologyId, string userId)
        {
            //context.UserTechnologies.Add(new UserTechnology
            //{
            //    TechnologyId = technologyId,
            //    Id = userId,
            //    ResearchStartDate = DateTime.Now,
            //    StatusId = UserTechnologyStatusTypes.Pending,
            //});
        }
    }
}