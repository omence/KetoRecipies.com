using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KetoRecipies.Data;
using KetoRecipies.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace KetoRecipies.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly KetoDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoriteController(KetoDbContext context, UserManager<ApplicationUser> userManager)
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
        public async Task<IActionResult> Favorite(string SearchString)
        {
            var userId = _userManager.GetUserId(User);
            var favs = await _context.favorites.Where(f => f.UserID == userId).ToListAsync();

            foreach (var f in favs)
            {
                f.Recipe = _context.recipes.FirstOrDefault(r => r.ID == f.RecipeID);
                f.Recipe.LikeCount = _context.Likes.Where(l => l.RecipeId == f.Recipe.ID && l.Liked == true).Count();
                f.Recipe.DisLikeCount = _context.Likes.Where(l => l.RecipeId == f.Recipe.ID && l.Liked == false).Count();
                
            }

            //search keyword from users favorite page
            if (!String.IsNullOrEmpty(SearchString))
            {
                var recipes1 = favs.Where(r => r.Recipe.Label.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0);
                var recipes2 = favs.Where(r => r.Recipe.Ingridients.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0);
                recipes1 = recipes1.Concat(recipes2);
                var splitSearch = SearchString.Split(" ");

                foreach (var i in splitSearch)
                {
                    var temp = favs.Where(r => r.Recipe.Label.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0);
                    var temp2 = favs.Where(r => r.Recipe.Ingridients.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0);
                    recipes1 = recipes1.Union(temp);
                    recipes1 = recipes1.Union(temp2);

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
        public IActionResult Create(int id, string userId)
        {
            if (id != 0)
            {
                Favorite favorite = new Favorite();
                favorite.UserID = userId;
                favorite.RecipeID = id;
                _context.favorites.Add(favorite);
                _context.SaveChanges();

                return RedirectToAction("Favorite");
            }

            return RedirectToAction("Favorite");
        }

        /// <summary>
        /// Removes Recipe from Favorites list
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Redirects to Index</returns>
        public IActionResult Remove(int id, int eleId, int count)
        {
            if (id != 0)
            {
                var toRemove = _context.favorites.FirstOrDefault(f => f.ID == id);
                _context.favorites.Remove(toRemove);
                _context.SaveChanges();

                if (eleId == count)
                {
                    return Redirect(Url.Action("Favorite") + $"#{eleId - 2}");
                }

                return Redirect(Url.Action("Favorite") + $"#{eleId--}");
            }
            return RedirectToAction("Favorite");
        }

        public IActionResult Details(int ID)
        {
            return RedirectToAction("Details", "Home", new { ID });
        }
    }
}
    