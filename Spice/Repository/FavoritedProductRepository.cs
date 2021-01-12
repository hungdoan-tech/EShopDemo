using Spice.Data;
using Spice.Models;
using Spice.Repository.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Repository
{
    public class FavoritedProductRepository: GenericRepository<FavoritedProduct>, IFavoritedProductRepository
    {
        public FavoritedProductRepository(ApplicationDbContext context) : base(context)
        {

        }

        public FavoritedProduct findByItemIdAndUserId(int itemId, string userId)
        {
            return this.dbSet.FirstOrDefault(a => a.ItemId == itemId && a.UserId == userId);
        }
    }
}
