using System.Collections.Generic;

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
