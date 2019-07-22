using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KetoRecipies.Data;
using KetoRecipies.Models;
using KetoRecipies.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace KetoRecipies.Controllers
{
    [Authorize]
    public class UserDashboardController : Controller
    {
        private IConfiguration _configuration;
        private readonly KetoDbContext _context;
        private readonly KetoRecipiesContext _users;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _he;
        private readonly IEmailSender _es;

        public UserDashboardController(IConfiguration configuration,
            KetoDbContext context,
            UserManager<ApplicationUser> userManager,
            IHostingEnvironment he,
            IEmailSender es,
            KetoRecipiesContext users)
        {
            _configuration = configuration;
            _context = context;
            _userManager = userManager;
            _he = he;
            _es = es;
            _users = users;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var recipes = _context.recipes.Where(r => r.UserId == userId).ToList();
            foreach (var rec in recipes)
            {
                rec.Comments = _context.mainComments.Where(c => c.RecipeID == rec.ID).ToList();
                foreach (var sc in rec.Comments)
                {
                    sc.SubComments = _context.subComments.Where(s => s.MainCommentID == sc.ID).ToList();
                }
                rec.LikeCount = _context.Likes.Where(c => c.RecipeId == rec.ID && c.Liked == true).Count();
                rec.DisLikeCount = _context.Likes.Where(c => c.RecipeId == rec.ID && c.Liked == false).Count();
            }

            UserDashboardViewModel UDVM = new UserDashboardViewModel();

            UDVM.MyRecipesCount = recipes.Count();
            UDVM.TotalComments = recipes.Sum(c => c.Comments.Count()) + recipes.Sum(s => s.Comments.Sum(sc => sc.SubComments.Count()));
            UDVM.TotalFavorites = _context.favorites.Where(f => f.UserID == userId).Count();
            UDVM.TotalLikes = recipes.Sum(l => l.LikeCount);

            return View(UDVM);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string Password, string NewPassword, string ConfirmPassword)
        {
            var user = await _userManager.GetUserAsync(User);

            if (NewPassword == ConfirmPassword)
            {
                await _userManager.ChangePasswordAsync(user, Password, NewPassword);

                return RedirectToAction("Success");
            }

            return RedirectToAction("Index");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}