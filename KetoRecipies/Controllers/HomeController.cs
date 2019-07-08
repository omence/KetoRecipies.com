using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KetoRecipies.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Text;
using KetoRecipies.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;

namespace KetoRecipies.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;
        private readonly KetoDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(IConfiguration configuration, KetoDbContext context, UserManager<IdentityUser> userManager)
        {
            _configuration = configuration;
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Send Index with recipes to View
        /// </summary>
        /// <returns>View + Recipe list</returns>
        public async Task<IActionResult> Index(int? page)
        {
            //await GetRecipes();
            var recipes = _context.recipes.OrderBy(r => r.Label).ToList();
            int pageSize = 27;
            int pageNumber = (page ?? 1);
            return View(recipes.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Searches recipe by keyword from user
        /// </summary>
        /// <param name="SearchString"></param>
        /// <returns>view + searched list</returns>
        [HttpPost]
        public async Task<IActionResult> Index(string SearchString, int? page)
        {
            var recipes = _context.recipes.ToList();

            if (!String.IsNullOrEmpty(SearchString))
            {
                var recipes1 = recipes.Where(r => r.Label.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                var recipes2 = recipes.Where(r => r.Ingridients.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                var recipes3 = recipes.Where(r => r.Source.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                recipes1 = recipes1.Union(recipes2).ToList();
                recipes1 = recipes1.Union(recipes3).ToList();

                var splitSearch = SearchString.Split(" ");

                foreach (var i in splitSearch)
                {
                    var temp = recipes.Where(r => r.Label.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                    var temp2 = recipes.Where(r => r.Ingridients.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                    recipes1 = recipes1.Union(temp).ToList();
                    recipes1 = recipes1.Union(temp2).ToList();

                }
                int pageSize = 100;
                int pageNumber = (page ?? 1);
        
                return View(recipes1.ToPagedList(pageNumber, pageSize));
            }
            int pageSize2 = 27;
            int pageNumber2 = (page ?? 1);
            return View(recipes.ToPagedList(pageNumber2, pageSize2));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// API Call to get recipes
        /// </summary>
        /// <returns></returns>
        public async Task<Recipe> GetRecipes()
        {
            using (var client = new HttpClient())
            {
                var ID = "04c323bd";
                var API = "164450c11102f2cb04cc1fe58795f157";
                try
                {
                    //call made to the api
                    client.BaseAddress = new Uri($"https://api.edamam.com/search");
                    string search = "low carb";

                    var response = await client.GetAsync($"?q={search}&app_id={ID}&app_key={API}&from=0&to=100");

                    response.EnsureSuccessStatusCode();

                    //Reades JSON file received from API
                    string result = await response.Content.ReadAsStringAsync();

                    dynamic recipes = JsonConvert.DeserializeObject(result);
                    //Build object
                    List<Recipe> list = new List<Recipe>();

                    foreach (var r in recipes.hits)
                    {

                        Recipe rec = new Recipe();
                        rec.ImageUrl = r.recipe.image;
                        rec.Label = r.recipe.label;
                        rec.Source = r.recipe.source;
                        rec.SourceUrl = r.recipe.url;
                        rec.Yield = r.recipe.yield;
                        StringBuilder sb = new StringBuilder();
                        foreach (var i in r.recipe.ingredientLines.ToObject<List<string>>())
                        {
                            sb.Append($"{i}; ");
                        }
                        rec.Ingridients = sb.ToString();
                        rec.TotalTime = r.recipe.totalTime;
                        rec.TotalCarbsServ = r.recipe.totalNutrients.CHOCDF.quantity / rec.Yield;
                        rec.TotalFatServ = r.recipe.totalNutrients.FAT.quantity / rec.Yield;
                        rec.TotalCaloriesServ = r.recipe.totalNutrients.ENERC_KCAL.quantity / rec.Yield;

                        var check = _context.recipes.FirstOrDefault(reci => reci.SourceUrl == rec.SourceUrl);

                        if (check == null)
                        {
                            _context.recipes.Add(rec);
                            _context.SaveChanges();
                        }
                    }
                }
                catch
                {
                    Recipe rec = new Recipe();
                    return rec;
                }
                Recipe rec2 = new Recipe();
                return rec2;
            }
        }

        /// <summary>
        /// Directs to MyFavorites Page
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        public IActionResult Favorite()
        {
            return RedirectToAction("Index", "Favorite");
        }

        /// <summary>
        /// Add a Recipe to the MyFavorites Page
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View</returns>
        [Authorize]
        public async Task<IActionResult>AddFavorite(int id)
        {
            var userId = _userManager.GetUserId(User);
            var checkForDupe = _context.favorites.FirstOrDefault(f => f.UserID == userId && f.RecipeID == id);

            if (checkForDupe == null)
            {
                FavoriteController fc = new FavoriteController(_context, _userManager);
                await fc.Create(id, userId);

                return RedirectToAction("Index", "Favorite");
            }

            TempData["Error"] = "Recipe already in your favorites list";
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(string Label, string Ingridients, string Instructions, string Source, string SourceUrl, decimal Yield, decimal TotalTime, decimal TotalCarbsServ, decimal TotalFatServ, decimal TotalCaloriesServ, string ImageUrl, string VideoUrl)
        {
            Recipe recipe = new Recipe();
            recipe.Label = Label;
            recipe.Ingridients = Ingridients;
            recipe.Instructions = Instructions;
            recipe.Source = Source;
            recipe.Yield = Yield;
            recipe.TotalTime = TotalTime;
            recipe.TotalCarbsServ = TotalCarbsServ;
            recipe.TotalFatServ = TotalFatServ;
            recipe.TotalCaloriesServ = TotalCaloriesServ;
            recipe.ImageUrl = ImageUrl;
            recipe.VideoUrl = VideoUrl;

            _context.recipes.Add(recipe);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int ID)
        {
            var recipe = _context.recipes.FirstOrDefault(r => r.ID == ID);

            return View(recipe);
        }

        public IActionResult Edit(int ID)
        {
            var recipe = _context.recipes.FirstOrDefault(r => r.ID == ID);

            return View(recipe);
        }
    }
}
