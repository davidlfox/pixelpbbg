using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PixelApp.Migrations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace PixelApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Index(IsUnique = true)]
        public override string UserName { get; set; }
        [Index(IsUnique = true)]
        public override string Email { get; set; }
        /// <summary>
        /// User energy
        /// </summary>
        public int Energy { get; set; }
        public int MaxEnergy { get; set; }
        public DateTime? EnergyUpdatedTime { get; set; }

        /// <summary>
        /// Player life/health/hit points
        /// </summary>
        public int Life { get; set; }
        public int MaxLife { get; set; }
        public DateTime? LifeUpdatedTime { get; set; }

        /// <summary>
        /// Player level
        /// </summary>
        public byte Level { get; set; }
        /// <summary>
        /// Player experience
        /// </summary>
        public int Experience { get; set; }

        // resource quantities

        /// <summary>
        /// Quantity of wood player is holding
        /// </summary>
        public int Wood { get; set; }

        /// <summary>
        /// Quantity of water player is holding
        /// </summary>
        public int Water { get; set; }

        /// <summary>
        /// Quantity of food player is holding
        /// </summary>
        public int Food { get; set; }

        /// <summary>
        /// Quantity of stone player is holding
        /// </summary>
        public int Stone { get; set; }

        /// <summary>
        /// Quantity of oil player is holding
        /// </summary>
        public int Oil { get; set; }

        /// <summary>
        /// Quantity of iron player is holding
        /// </summary>
        public int Iron { get; set; }

        /// <summary>
        /// The player's home territory
        /// </summary>
        [ForeignKey("Territory")]
        public int? TerritoryId { get; set; }
        public virtual Territory Territory { get; set; }

        /// <summary>
        /// Collection of nightly attacks on the user's territory
        /// </summary>
        public ICollection<AttackLog> AttackLogs { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<PixelApp.Models.Territory> Territories { get; set; }

        public System.Data.Entity.DbSet<PixelApp.Models.Technology> Technologies { get; set; }

        public System.Data.Entity.DbSet<PixelApp.Models.AttackLog> AttackLogs { get; set; }

        public System.Data.Entity.DbSet<PixelApp.Models.Trade> Trades { get; set; }
    }
}