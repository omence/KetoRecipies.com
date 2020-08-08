namespace KetoRecipies.Models
{
    public class Like
    {
        public int ID { get; set; }

        public int RecipeId { get; set; }

        public string UserId { get; set; }

        public bool Liked { get; set; }
    }
}
