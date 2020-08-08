using KetoRecipies.Data;
using KetoRecipies.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KetoRecipies.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly KetoDbContext _context;
        private readonly KetoRecipiesContext _users;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _he;


        public AdminController(KetoDbContext context,
            UserManager<ApplicationUser> userManager,
            KetoRecipiesContext users,
            IHostingEnvironment he)
        {
            _context = context;
            _userManager = userManager;
            _users = users;
            _he = he;
        }

        /// <summary>
        /// sends Index page
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Sends list of all users
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _users.Users.ToListAsync();
            return View(users);
        }

        /// <summary>
        /// Sends a list of all recipes
        /// </summary>
        /// <param name="SearchString"></param>
        /// <returns></returns>
        public async Task<IActionResult> ManageRecipes(string SearchString)
        {
            var recipes = await _context.recipes.ToListAsync();
            if (!String.IsNullOrEmpty(SearchString))
            {
                var recipes1 = recipes.Where(r => r.Label.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0);
                var recipes2 = recipes.Where(r => r.Ingridients.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0);
                var recipes3 = recipes.Where(r => r.Source.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0);
                recipes1 = recipes1.Union(recipes2);
                recipes1 = recipes1.Union(recipes3);
                var splitSearch = SearchString.Split(" ");

                foreach (var i in splitSearch)
                {
                    var temp = recipes.Where(r => r.Label.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0);
                    var temp2 = recipes.Where(r => r.Ingridients.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0);
                    recipes1 = recipes1.Union(temp);
                    recipes1 = recipes1.Union(temp2);

                }
                TempData["SearchString"] = SearchString;
                return View(recipes1);
            }
            return View(recipes);
        }

        /// <summary>
        /// sends edit page for recipe
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AdminEditRecipe(int ID)
        {
            return View(_context.recipes.FirstOrDefault(r => r.ID == ID));
        }

        /// <summary>
        /// Takes in input and updates Recipe
        /// </summary>
        /// <returns>Manage recipe view</returns>
        [HttpPost]
        public IActionResult AdminEditRecipe(int ID, string Type, string UserId, string Label, string Ingridients, string Instructions, string Source, string SourceUrl, decimal Yield, decimal TotalTime, decimal TotalCarbsServ, decimal TotalFatServ, decimal TotalCaloriesServ, IFormFile ImageUrl, string VideoUrl)
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
            recipe.Type = Type;
            recipe.Label = Label;
            recipe.Ingridients = Ingridients;
            recipe.Instructions = Instructions;
            recipe.Source = Source;
            recipe.Yield = Convert.ToInt32(Yield);
            recipe.TotalTime = Convert.ToInt32(TotalTime);
            recipe.TotalCarbsServ = Convert.ToInt32(TotalCarbsServ);
            recipe.TotalFatServ = Convert.ToInt32(TotalFatServ);
            recipe.TotalCaloriesServ = Convert.ToInt32(TotalCaloriesServ);
            recipe.VideoUrl = VideoUrl;

            _context.recipes.Update(recipe);
            _context.SaveChanges();

            return Redirect(Url.Action("ManageRecipes") + $"#{Label}");
        }

        /// <summary>
        /// Sends are you sure view
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public IActionResult AdminDeleteRecipe(string searchString, int ID)
        {
            TempData["SearchString"] = searchString;
            return View(_context.recipes.FirstOrDefault(r => r.ID == ID));
        }

        /// <summary>
        /// Deletes Recipe from the Database
        /// </summary>
        /// <param name="ID"></param>
        /// <returns>Manage Recipes view</returns>
        [HttpPost]
        public IActionResult AdminDeleteRecipe2(string searchString, int ID)
        {
            var toDelete = _context.recipes.FirstOrDefault(r => r.ID == ID);
            _context.recipes.Remove(toDelete);
            _context.SaveChanges();

            return Redirect(Url.Action("ManageRecipes", new { searchString }));
        }
    }
}