using KetoRecipies.Data;
using KetoRecipies.Models.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KetoRecipies.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly KetoDbContext _context;

        public CommentController(KetoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Add a main comment to DB
        /// </summary>
        /// <param name="mainComment"></param>
        public void CreateMainComment(MainComment mainComment)
        {
            _context.mainComments.Add(mainComment);
            _context.SaveChanges();
        }

        /// <summary>
        /// Adds a subcomment to the DB
        /// </summary>
        /// <param name="subComment"></param>
        public void CreateSubComment(SubComment subComment)
        {
            _context.subComments.Add(subComment);
            _context.SaveChanges();
        }
    }
}