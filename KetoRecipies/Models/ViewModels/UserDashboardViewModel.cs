using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KetoRecipies.Models.ViewModels
{
    public class UserDashboardViewModel
    {
        public int MyRecipesCount { get; set; }

        public int TotalLikes { get; set; }

        public int TotalComments { get; set; }

        public int TotalFavorites { get; set; }


    }
}
