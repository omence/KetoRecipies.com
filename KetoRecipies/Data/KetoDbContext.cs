using KetoRecipies.Models;
using KetoRecipies.Models.Comments;
using Microsoft.EntityFrameworkCore;

namespace KetoRecipies.Data
{
    public class KetoDbContext : DbContext
    {
        public KetoDbContext(DbContextOptions<KetoDbContext> options) : base(options)
        {

        }

        public DbSet<Recipe> recipes { get; set; }
        public DbSet<Favorite> favorites { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<MainComment> mainComments { get; set; }
        public DbSet<SubComment> subComments { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
