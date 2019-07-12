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
    [Authorize]
    public class LikeController : Controller
    { 
        private readonly KetoDbContext _context;

        public LikeController(KetoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Allows a user to like a Recipe
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="userId"></param>
        /// <returns>View</returns>
        public IActionResult Like(int ID, string userId)
        {
            var checkForLike = _context.Likes.FirstOrDefault(l => l.UserId == userId && l.RecipeId == ID);

            //if a Like does not exist, create one
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
            //If Like does exist modify it
            checkForLike.Liked = true;
            _context.Likes.Update(checkForLike);
            _context.SaveChanges();

            return RedirectToAction("Details", "Home", new { ID });
        }

        /// <summary>
        /// Allows a user to dislike a Recipe
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IActionResult DisLike(int ID, string userId)
        {
            var checkForLike = _context.Likes.FirstOrDefault(l => l.UserId == userId && l.RecipeId == ID);
            //if Like does not exist create one
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
            //If like does exist, modify it
            checkForLike.Liked = false;
            _context.Likes.Update(checkForLike);
            _context.SaveChanges();

            return RedirectToAction("Details", "Home", new { ID });
        }
    }
}