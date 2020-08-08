using System;

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
