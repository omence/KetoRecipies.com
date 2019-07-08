using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KetoRecipies.Models
{
    public class Recipe
    {
        public int ID { get; set; }

        public string UserId { get; set; }

        public string Label { get; set; }

        public string Ingridients { get; set; }

        public string Instructions { get; set; }

        public string Source { get; set; }

        public string SourceUrl { get; set; }

        public decimal Yield { get; set; }

        public decimal TotalTime { get; set; }

        public decimal TotalCarbsServ { get; set; }

        public decimal TotalFatServ { get; set; }

        public decimal TotalCaloriesServ { get; set; }

        public string ImageUrl { get; set; }

        public string VideoUrl { get; set; }
    }
}
