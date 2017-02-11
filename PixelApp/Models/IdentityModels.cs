﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PixelApp.Migrations;
using System;

namespace PixelApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
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
    }
}