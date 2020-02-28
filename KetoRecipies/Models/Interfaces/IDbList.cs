using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KetoRecipies.Models.Interfaces
{
    public interface IDbList
    {
        //Gets List of all Favorites
        List<Favorite> GetFavorites();

        //Gets a list of rendom recipes
        List<Product> GetRandomProducts();
    } 
}
