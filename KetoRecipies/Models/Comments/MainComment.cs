using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KetoRecipies.Models.Comments
{
    public class MainComment : Comment
    {
        public int RecipeID { get; set; }

        public List<SubComment> SubComments { get; set; }
    }
}
