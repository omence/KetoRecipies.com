using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KetoRecipies.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string ProductUrl { get; set; }

        public string ImageUrl { get; set; }

        public string ProductType { get; set; }
    }
}
