using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KetoRecipies.Data;
using KetoRecipies.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageUsers()
        {
            var users = _users.Users.ToList();
            return View(users);
        }

        public IActionResult ManageRecipes()
        {
            return View(_context.recipes.ToList());
        }

        public IActionResult AdminEditRecipe(int ID)
        {
            return View(_context.recipes.FirstOrDefault(r => r.ID == ID));
        }

        [HttpPost]
        public IActionResult AdminEditRecipe(int ID, string UserId, string Label, string Ingridients, string Instructions, string Source, string SourceUrl, decimal Yield, decimal TotalTime, decimal TotalCarbsServ, decimal TotalFatServ, decimal TotalCaloriesServ, IFormFile ImageUrl, string VideoUrl)
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

            return Redirect(Url.Action("ManageRecipes") + $"#{Label}");
        }

        public IActionResult AdminDeleteRecipe(int ID)
        {
            return View(_context.recipes.FirstOrDefault(r => r.ID == ID));
        }

        [HttpPost]
        public IActionResult AdminDeleteRecipe2(int ID)
        {
            var toDelete = _context.recipes.FirstOrDefault(r => r.ID == ID);
            _context.recipes.Remove(toDelete);
            _context.SaveChanges();

            return RedirectToAction("ManageRecipes");
        }
    }
}