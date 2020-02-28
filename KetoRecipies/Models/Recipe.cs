using KetoRecipies.Models.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KetoRecipies.Models
{
    public class Recipe
    {
        public int ID { get; set; }

        public DateTime DateAdded { get; set; }

        public bool IncludeSocialMediaLinks { get; set; }

        public string Facebook { get; set; }
        public string YouTube { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }

        public string UserId { get; set; }

        public string Type { get; set; }

        public string Label { get; set; }

        public string Ingridients { get; set; }

        public string Instructions { get; set; }

        public string Source { get; set; }

        public string SourceUrl { get; set; }

        public int Yield { get; set; }

        public int TotalTime { get; set; }

        public int TotalCarbsServ { get; set; }

        public int TotalFatServ { get; set; }

        public int TotalCaloriesServ { get; set; }

        public string ImageUrl { get; set; }

        public string VideoUrl { get; set; }

        public int LikeCount { get; set; }

        public int DisLikeCount { get; set; }

        public List<MainComment> Comments { get; set; }

        public int ViewCount { get; set; }
    }
}
