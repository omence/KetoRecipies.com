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

        public DbListService(KetoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all Favorites from DB
        /// </summary>
        /// <returns>List of Favorites</returns>
        public List<Favorite> GetFavorites()
        {
            return (_context.favorites.ToList());
        }

        /// <summary>
        /// Gets reandom recipes from DB
        /// </summary>
        /// <returns>A List of random recipes</returns>
        public List<Product> GetRandomProducts()
        {
            Random rnd = new Random();

            return (_context.Products.OrderBy(r => rnd.Next()).Take(16).ToList());
        }
    }
}
