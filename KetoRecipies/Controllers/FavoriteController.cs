using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KetoRecipies.Data;
using KetoRecipies.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KetoRecipies.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly KetoDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FavoriteController(KetoDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// User can save favorite recipes
        /// </summary>
        /// <param name="SearchString"></param>
        /// <returns>Favorite views with recipes</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index(string SearchString)
        {
            var userId = _userManager.GetUserId(User);
            var favs = _context.favorites.Where(f => f.UserID == userId).ToList();

            foreach (var f in favs)
            {
                f.Recipe = _context.recipes.FirstOrDefault(r => r.ID == f.RecipeID);
                f.Recipe.LikeCount = _context.Likes.Where(l => l.RecipeId == f.Recipe.ID && l.Liked == true).Count();
                f.Recipe.DisLikeCount = _context.Likes.Where(l => l.RecipeId == f.Recipe.ID && l.Liked == false).Count();
                
            }

            //search keyword from users favorite page
            if (!String.IsNullOrEmpty(SearchString))
            {
                var recipes1 = favs.Where(r => r.Recipe.Label.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                var recipes2 = favs.Where(r => r.Recipe.Ingridients.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                recipes1 = recipes1.Concat(recipes2).ToList();
                var splitSearch = SearchString.Split(" ");

                foreach (var i in splitSearch)
                {
                    var temp = favs.Where(r => r.Recipe.Label.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                    var temp2 = favs.Where(r => r.Recipe.Ingridients.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                    recipes1 = recipes1.Union(temp).ToList();
                    recipes1 = recipes1.Union(temp2).ToList();

                }

                return View(recipes1);
            }
            return View(favs);
        }

        /// <summary>
        /// Add a recipe to users favorites
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View and Favorite Recipe List</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(int id, string userId)
        {
            if (id != 0)
            {
                Favorite favorite = new Favorite();
                favorite.UserID = userId;
                favorite.RecipeID = id;
                _context.favorites.Add(favorite);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Removes Recipe from Favorites list
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Redirects to Index</returns>
        public IActionResult Remove(int id)
        {
            if (id != 0)
            {
                var toRemove = _context.favorites.FirstOrDefault(f => f.ID == id);
                _context.favorites.Remove(toRemove);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
    