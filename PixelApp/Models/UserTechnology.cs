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

        [ForeignKey("TechnologyId")]
        public int TechnologyId { get; set; }

        virtual public Technology Technology { get; set; }

        [ForeignKey("Id")]
        public Guid Id { get; set; }

        virtual public ApplicationUser ApplicationUser { get; set; }

        public DateTime ResearchStartDate { get; set; }         
        
        public UserTechnologyStatusTypes StatusId { get; set; }   
    }

    public enum UserTechnologyStatusTypes
    {
        Pending,
        Researched,
    }
}