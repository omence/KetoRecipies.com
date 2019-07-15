using KetoRecipies.Data;
using KetoRecipies.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KetoRecipies.Models.Service
{
    public class DbListService : IDbList
    {
        private readonly KetoDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DbListService(KetoDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Favorite> GetFavorites()
        {
            return (_context.favorites.ToList());
        }

        public List<Recipe> GetRandomRecipes()
        {
            Random rnd = new Random();

            return (_context.recipes.OrderBy(r => rnd.Next()).Take(100).ToList());
        }
    }
}
