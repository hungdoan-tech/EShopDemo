using Spice.Models;
using Spice.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Repository.RepositoryInterface
{
    public interface IFavoritedProductRepository : IRepository<FavoritedProduct>
    {
        FavoritedProduct findByItemIdAndUserId(int itemId, string userId);
    }
}
