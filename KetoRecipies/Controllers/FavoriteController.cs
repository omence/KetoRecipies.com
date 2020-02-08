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
        public async Task<IActionResult> Favorite(string SearchString, string filter, string sort, int? page)
        {
            var userId = _userManager.GetUserId(User);
            var recipes = await _context.favorites.Where(f => f.UserID == userId).ToListAsync();

            foreach (var f in recipes)
            {
                f.Recipe = _context.recipes.FirstOrDefault(r => r.ID == f.RecipeID);
                f.Recipe.LikeCount = _context.Likes.Where(l => l.RecipeId == f.Recipe.ID && l.Liked == true).Count();
                f.Recipe.DisLikeCount = _context.Likes.Where(l => l.RecipeId == f.Recipe.ID && l.Liked == false).Count();
                
            }

            if (!string.IsNullOrEmpty(filter))
            {
                TempData["filter"] = filter;
                if (filter == "Breakfast")
                {
                    recipes = recipes.Where(r => r.Recipe.Type == "Breakfast").ToList();
                }
                if (filter == "Lunch")
                {
                    recipes = recipes.Where(r => r.Recipe.Type == "Lunch").ToList();
                }
                if (filter == "Dinner")
                {
                    recipes = recipes.Where(r => r.Recipe.Type == "Dinner").ToList();
                }
                if (filter == "Side")
                {
                    recipes = recipes.Where(r => r.Recipe.Type == "Side").ToList();
                }
                if (filter == "Dessert")
                {
                    recipes = recipes.Where(r => r.Recipe.Type == "Dessert").ToList();
                }
                if (filter == "Snack")
                {
                    recipes = recipes.Where(r => r.Recipe.Type == "Snack").ToList();
                }
                if (filter == "Drink")
                {
                    recipes = recipes.Where(r => r.Recipe.Type == "Drink").ToList();
                }
            }
            if (!String.IsNullOrEmpty(SearchString))
            {
                var filteredList = recipes.Where(r => r.Recipe.Label.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0 || SpellCompare(r.Recipe.Label, SearchString) < 3 ||
                r.Recipe.Ingridients.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0 ||
                r.Recipe.Source.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0 || SpellCompare(r.Recipe.Source, SearchString) < 3).ToList();

                var splitSearch = SearchString.Split(" ");

                foreach (var i in splitSearch)
                {
                    if (i.ToLower() != "keto")
                    {
                        var temp = recipes.Where(r => r.Recipe.Label.IndexOf(i, StringComparison.OrdinalIgnoreCase) >= 0 || SpellCompare(r.Recipe.Label, i) < 3 ||
                        r.Recipe.Ingridients.IndexOf(i, StringComparison.OrdinalIgnoreCase) >= 0 || SpellCompare(r.Recipe.Ingridients, i) < 3 ||
                        r.Recipe.Source.IndexOf(i, StringComparison.OrdinalIgnoreCase) >= 0 || SpellCompare(r.Recipe.Source, i) < 3).ToList();

                        recipes = filteredList.Union(temp).ToList();
                    }
                }

            }
            if (!string.IsNullOrEmpty(sort))
            {
                if (sort == "mostLikes")
                {
                    recipes = recipes.OrderByDescending(r => r.Recipe.LikeCount).ToList();
                }
                if (sort == "newest")
                {
                    recipes = recipes.OrderByDescending(r => r.Recipe.DateAdded).ToList();
                }
                if (sort == "oldest")
                {
                    recipes = recipes.OrderBy(r => r.Recipe.DateAdded).ToList();
                }
            }
            TempData["SearchString"] = SearchString;
            TempData["sort"] = sort;
            int pageSize = 27;
            int pageNumber = (page ?? 1);
            return View(recipes.ToPagedList(pageNumber, pageSize));
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

        public static int SpellCompare(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }
}
    