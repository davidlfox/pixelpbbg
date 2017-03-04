using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelApp.Views.Home.Models
{
    public class UsersViewModel
    {
        public List<UserSkinny> Users { get; set; }
        public int UserCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public bool ShowNextButton { get; set; }

        public UsersViewModel()
        {
            this.Users = new List<UserSkinny>();
        }
    }

    public class UserSkinny
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public int Level { get; set; }
        public string TerritoryName { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string TerrainType { get; set; }
    }
}