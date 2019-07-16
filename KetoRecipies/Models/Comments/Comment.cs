using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KetoRecipies.Models.Comments
{
    public class Comment
    {
        public int ID { get; set; }

        public string Message { get; set; }

        public DateTime DateTime { get; set; }

        public string User { get; set; }
    }
}
