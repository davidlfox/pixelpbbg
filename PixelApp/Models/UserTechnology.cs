using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PixelApp.Models
{
    public class UserTechnology
    {
        [Key]
        public int UserTechnologyId { get; set; }

        [ForeignKey("Technology")]
        public int TechnologyId { get; set; }

        public virtual Technology Technology { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        virtual public ApplicationUser User { get; set; }

        public DateTime ResearchStartDate { get; set; }  
        
        public int ResearchDays { get; set; }       
        
        public UserTechnologyStatusTypes StatusId { get; set; }   
    }

    public enum UserTechnologyStatusTypes
    {
        Pending,
        Researched,
    }
}