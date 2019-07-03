using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KetoRecipies.Models
{
    public class Favorite
    {
        public int ID { get; set; }

        public string UserID { get; set; }

        public int RecipeID { get; set; }

        public Recipe Recipe { get; set; }
    }
}
