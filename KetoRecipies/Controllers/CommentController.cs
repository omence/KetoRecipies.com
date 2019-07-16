using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task CreateMainComment(MainComment mainComment)
        {
            _context.mainComments.Add(mainComment);
            _context.SaveChanges();
        }

        public async Task CreateSubComment(SubComment subComment)
        {
            _context.subComments.Add(subComment);
            _context.SaveChanges();
        }
    }
}