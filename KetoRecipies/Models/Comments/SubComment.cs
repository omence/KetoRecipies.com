using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KetoRecipies.Models.Comments
{
    public class SubComment : Comment
    {
        public int MainCommentID { get; set; }
    }
}
