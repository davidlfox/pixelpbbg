using Pixel.Common.Data;
using PixelApp.Models;
using System.Collections.Generic;

namespace PixelApp.Views.Map.Models
{
    public class AttackTerritoryModel
    {
        public AttackTerritoryModel()
        {
            ResultMessages = new Dictionary<string, object>();
        }

        public string TerritoryName { get; set; }

        public TerritoryTypes TerritoryTypeId { get; set; }

        public string UserName { get; set; }

        public int Level { get; set; }

        public int TerritoryId { get; set; }

        public int Population { get; set; }

        public Dictionary<string, object> ResultMessages { get; set; }
    }
}