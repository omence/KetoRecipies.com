using KetoRecipies.Models;
using KetoRecipies.Models.Comments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
