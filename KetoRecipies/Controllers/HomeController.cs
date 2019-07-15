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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace KetoRecipies.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;
        private readonly KetoDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHostingEnvironment _he;
        private readonly IEmailSender _es;

        public HomeController(IConfiguration configuration, KetoDbContext context, UserManager<IdentityUser> userManager, IHostingEnvironment he, IEmailSender es)
        {
            _configuration = configuration;
            _context = context;
            _userManager = userManager;
            _he = he;
            _es = es;
        }

        /// <summary>
        /// Send Index with recipes to View
        /// </summary>
        /// <returns>View + Recipe list</returns>
        public async Task<IActionResult> Index(int? page)
        {
            //await GetRecipes();
            var recipes = _context.recipes.OrderBy(r => r.Label).ToList();
            foreach (var i in recipes)
            {
                i.LikeCount = _context.Likes.Where(l => l.RecipeId == i.ID && l.Liked == true).Count();
                i.DisLikeCount = _context.Likes.Where(l => l.RecipeId == i.ID && l.Liked == false).Count();
            }
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
            var recipes = _context.recipes.OrderBy(r => r.Label).ToList();
            foreach (var i in recipes)
            {
                i.LikeCount = _context.Likes.Where(l => l.RecipeId == i.ID && l.Liked == true).Count();
                i.DisLikeCount = _context.Likes.Where(l => l.RecipeId == i.ID && l.Liked == false).Count();
            }

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
                int pageSize = 27;
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
                    string search = "ketogenic";

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
        [HttpGet]
        public IActionResult Favorite()
        {
            return RedirectToAction("Favorite", "Favorite");
        }

        /// <summary>
        /// Add a Recipe to the MyFavorites Page
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddFavorite(int id)
        {
            var userId = _userManager.GetUserId(User);
            var checkForDupe = _context.favorites.FirstOrDefault(f => f.UserID == userId && f.RecipeID == id);
            var recipe = _context.recipes.FirstOrDefault(r => r.ID == id);
            if (checkForDupe == null)
            {
                FavoriteController fc = new FavoriteController(_context, _userManager);
                await fc.Create(id, userId);

                return Redirect(Url.Action("Index") + $"#{recipe.Label}");
            }

            TempData["Error"] = "Recipe already in your favorites list";
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFavorite(int id)
        {
            var userId = _userManager.GetUserId(User);
            var recipe = _context.recipes.FirstOrDefault(r => r.ID == id);
            var toRemove = _context.favorites.FirstOrDefault(f => f.RecipeID == id && f.UserID == userId);

            FavoriteController fc = new FavoriteController(_context, _userManager);
            fc.Remove(toRemove.ID);

            return Redirect(Url.Action("Index") + $"#{recipe.Label}");

        }

        /// <summary>
        /// Sends user to create view
        /// </summary>
        /// <returns>view</returns>
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Takes in user info and builds Recipe
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(string Label, string Ingridients, string Instructions, string Source, string SourceUrl, decimal Yield, decimal TotalTime, decimal TotalCarbsServ, decimal TotalFatServ, decimal TotalCaloriesServ, IFormFile ImageUrl, string VideoUrl)
        {
            var userId = _userManager.GetUserId(User);
            Recipe recipe = new Recipe();

            //save photo of dish and set ImageUrl property to location
            if (ImageUrl != null)
            {
                var fileName = Path.Combine($"{_he.WebRootPath}/Images", Path.GetFileName(ImageUrl.FileName));
                ImageUrl.CopyTo(new FileStream(fileName, FileMode.Create));
                recipe.ImageUrl = "/Images/" + Path.GetFileName(ImageUrl.FileName);
            }

            recipe.UserId = userId;
            recipe.Label = Label;
            recipe.Ingridients = Ingridients;
            recipe.Instructions = Instructions;
            recipe.Source = Source;
            recipe.Yield = Yield;
            recipe.TotalTime = TotalTime;
            recipe.TotalCarbsServ = TotalCarbsServ;
            recipe.TotalFatServ = TotalFatServ;
            recipe.TotalCaloriesServ = TotalCaloriesServ;
            recipe.VideoUrl = VideoUrl;

            _context.recipes.Add(recipe);
            _context.SaveChanges();

            return RedirectToAction("MyRecipes");
        }

        /// <summary>
        /// Gets Recipe by ID and sends to Details View
        /// </summary>
        /// <param name="ID"></param>
        /// <returns>View</returns>
        [HttpGet]
        public IActionResult Details(int ID)
        {
            var recipe = _context.recipes.FirstOrDefault(r => r.ID == ID);
            recipe.LikeCount = _context.Likes.Where(l => l.RecipeId == recipe.ID && l.Liked == true).Count();
            recipe.DisLikeCount = _context.Likes.Where(l => l.RecipeId == recipe.ID && l.Liked == false).Count();

            return View(recipe);
        }

        /// <summary>
        /// Gets recipe by ID and sends to Edit view
        /// </summary>
        /// <param name="ID"></param>
        /// <returns>View</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Edit(int ID)
        {
            var recipe = _context.recipes.FirstOrDefault(r => r.ID == ID);

            return View(recipe);
        }

        /// <summary>
        /// Takes in user info to edit Recipe object
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        [HttpPost]
        public IActionResult Edit(int ID, string UserId, string Label, string Ingridients, string Instructions, string Source, string SourceUrl, decimal Yield, decimal TotalTime, decimal TotalCarbsServ, decimal TotalFatServ, decimal TotalCaloriesServ, IFormFile ImageUrl, string VideoUrl)
        {
            var recipe = _context.recipes.FirstOrDefault(r => r.ID == ID);

            //save photo of dish and set ImageUrl property to location
            if (ImageUrl != null)
            {
                var fileName = Path.Combine($"{_he.WebRootPath}/Images", Path.GetFileName(ImageUrl.FileName));
                ImageUrl.CopyTo(new FileStream(fileName, FileMode.Create));
                recipe.ImageUrl = "/Images/" + Path.GetFileName(ImageUrl.FileName);
            }
            recipe.UserId = UserId;
            recipe.Label = Label;
            recipe.Ingridients = Ingridients;
            recipe.Instructions = Instructions;
            recipe.Source = Source;
            recipe.Yield = Yield;
            recipe.TotalTime = TotalTime;
            recipe.TotalCarbsServ = TotalCarbsServ;
            recipe.TotalFatServ = TotalFatServ;
            recipe.TotalCaloriesServ = TotalCaloriesServ;
            recipe.VideoUrl = VideoUrl;

            _context.recipes.Update(recipe);
            _context.SaveChanges();

            return RedirectToAction("MyRecipes");
        }

        /// <summary>
        /// Gets a list of users Resipes and sends to View
        /// </summary>
        /// <returns>View</returns>
        [HttpGet]
        [Authorize]
        public IActionResult MyRecipes()
        {
            var userId = _userManager.GetUserId(User);

            var recipes = _context.recipes.Where(r => r.UserId == userId).OrderBy(r => r.Label).ToList();
            foreach(var i in recipes)
            {
                i.LikeCount = _context.Likes.Where(l => l.RecipeId == i.ID && l.Liked == true).Count();
                i.DisLikeCount = _context.Likes.Where(l => l.RecipeId == i.ID && l.Liked == false).Count();
            }

            return View(recipes);
        }

        /// <summary>
        /// Sends user to Delete verification page when delete button is clicked
        /// </summary>
        /// <param name="ID"></param>
        /// <returns>View</returns>
        [HttpGet]
        [Authorize]
        public IActionResult DeleteAreYouSure(int ID)
        {
            if (ID > 0)
            {
                var delete = _context.recipes.FirstOrDefault(r => r.ID == ID);

                return View(delete);
            }
            return RedirectToAction("MyRecipes");
        }

        /// <summary>
        /// Deletes and Recipe
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public IActionResult Delete(int ID)
        {
            if (ID > 0)
            {
                var toDelete = _context.recipes.FirstOrDefault(r => r.ID == ID);
                _context.recipes.Remove(toDelete);
                _context.SaveChanges();

                return RedirectToAction("MyRecipes");
            }
            return RedirectToAction("MyRecipes");
        }

        /// <summary>
        /// Allows user to like a Recipe
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Authorize]
        public IActionResult Like(int ID)
        {
            LikeController lc = new LikeController(_context);
            var userId = _userManager.GetUserId(User);

            lc.Like(ID, userId);

            return Redirect(Url.Action("Details", "Home", new { ID }) + "#Here");
        }

        /// <summary>
        /// Allows a user to dislike a recipe
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Authorize]
        public IActionResult DisLike(int ID)
        {
            LikeController lc = new LikeController(_context);
            var userId = _userManager.GetUserId(User);

            lc.DisLike(ID, userId);

            return Redirect(Url.Action("Details", "Home", new { ID }) + "#Here");
           
        }

        /// <summary>
        /// Send ContactUs view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ContactUs()
        {
            return View();
        }

        /// <summary>
        /// Sends email to admin
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns>View</returns>
        [HttpPost]
        public IActionResult ContactUs(string email, string subject, string message)
        {
            if (email != null && subject != null && message != null)
            {
                string msg = $"{email} {message}";

                _es.SendEmailAsync("omence11@gmail.com", subject, msg);

                TempData["Message"] = "Sent, we will get back to you ASAP";

                return View();
            }

            return View();
        }
    }
}
