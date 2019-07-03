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
        public async Task<IActionResult> Index()
        {
            //await GetRecipes();
            var recipes = _context.recipes.OrderBy(r => r.Label).ToList();
            return View(recipes);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string SearchString)
        {
            var recipes = _context.recipes.ToList();

            if (!String.IsNullOrEmpty(SearchString))
            {
                var recipes1 = recipes.Where(r => r.Label.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                var recipes2 = recipes.Where(r => r.Ingridients.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                recipes1 = recipes1.Concat(recipes2).ToList();

                var splitSearch = SearchString.Split(" ");

                foreach (var i in splitSearch)
                {
                    var temp = recipes.Where(r => r.Label.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                    var temp2 = recipes.Where(r => r.Ingridients.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                    recipes1 = recipes1.Union(temp).ToList();

                    recipes1 = recipes1.Union(temp2).ToList();

                }

                return View(recipes1);
            }

            return View(recipes);
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

        public async Task<Recipe> GetRecipes()
        {
            using (var client = new HttpClient())
            {
                var ID = _configuration["ID"];
                var API = _configuration["RecipeApiKey"];
                try
                {
                    //call made to the api
                    client.BaseAddress = new Uri($"https://api.edamam.com/search");
                    string search = "Keto enchiladas";

                    var response = await client.GetAsync($"?q={search}&app_id={ID}&app_key={API}&from=0&to=100");

                    response.EnsureSuccessStatusCode();


                    //Reades JSON file received from API
                    string result = await response.Content.ReadAsStringAsync();

                    dynamic recipes = JsonConvert.DeserializeObject(result);
                    //Build object
                    List<Recipe> list = new List<Recipe>();

                    foreach(var r in recipes.hits) {

                        Recipe rec = new Recipe();
                        rec.ImageUrl = r.recipe.image;
                        rec.Label = r.recipe.label;
                        rec.Source = r.recipe.source;
                        rec.SourceUrl = r.recipe.url;
                        rec.Yield = r.recipe.yield;
                        StringBuilder sb = new StringBuilder();
                        foreach(var i in r.recipe.ingredientLines.ToObject<List<string>>())
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

        [HttpGet]
        public async Task<IActionResult> Favorite(string SearchString)
        {
            var userId = _userManager.GetUserId(User);

            var favs = _context.favorites.Where(f => f.UserID == userId).ToList();

            foreach(var f in favs)
            {
                f.Recipe = _context.recipes.FirstOrDefault(r => r.ID == f.RecipeID);
            }

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

        [HttpPost]
        public async Task<IActionResult> Favorite(int id)
        {
            if (id != null)
            {
                var userId2 = _userManager.GetUserId(User);

                Favorite favorite = new Favorite();

                favorite.UserID = userId2;
                favorite.RecipeID = id;

                _context.favorites.Add(favorite);
                _context.SaveChanges();

                var favs = _context.favorites.Where(f => f.UserID == userId2).ToList();

                foreach (var f in favs)
                {
                    f.Recipe = _context.recipes.FirstOrDefault(r => r.ID == f.RecipeID);
                }

                return View(favs);
            }

            var userId = _userManager.GetUserId(User);

            var favs2 = _context.favorites.Where(f => f.UserID == userId).ToList();

            foreach (var f in favs2)
            {
                f.Recipe = _context.recipes.FirstOrDefault(r => r.ID == f.RecipeID);
            }

            return View(favs2);
        }
    }
}
