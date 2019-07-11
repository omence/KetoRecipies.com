using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KetoRecipies.Data;
using KetoRecipies.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KetoRecipies.Controllers
{
    public class LikeController : Controller
    { 
        private readonly KetoDbContext _context;

        public LikeController(KetoDbContext context)
        {
            _context = context;
        }
        public IActionResult Like(int ID, string userId)
        {
            var checkForLike = _context.Likes.FirstOrDefault(l => l.UserId == userId && l.RecipeId == ID);

            if(checkForLike == null)
            {
                Like like = new Like();

                like.RecipeId = ID;
                like.UserId = userId;
                like.Liked = true;

                _context.Likes.Add(like);
                _context.SaveChanges();

                return RedirectToAction("Details", "Home", new { ID });

            }

            return RedirectToAction("Details", "Home", new { ID });
        }

        public IActionResult DisLike(int ID, string userId)
        {
            var checkForLike = _context.Likes.FirstOrDefault(l => l.UserId == userId && l.RecipeId == ID);

            if (checkForLike == null)
            {
                Like like = new Like();

                like.RecipeId = ID;
                like.UserId = userId;
                like.Liked = false;

                _context.Likes.Add(like);
                _context.SaveChanges();

                return RedirectToAction("Details", "Home", new { ID });

            }

            return RedirectToAction("Details", "Home", new { ID });
        }
    }
}