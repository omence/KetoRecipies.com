using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KetoRecipies.Models.Interfaces
{
    public interface IDbList
    {
        List<Favorite> GetFavorites();

        List<Recipe> GetRandomRecipes();
    }

   
}
