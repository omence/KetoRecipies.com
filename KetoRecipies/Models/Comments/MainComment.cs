using System.Collections.Generic;

namespace KetoRecipies.Models.Comments
{
    public class MainComment : Comment
    {
        public int RecipeID { get; set; }

        public List<SubComment> SubComments { get; set; }
    }
}
