using Spice.Models;
using Spice.Repository.RepositoryInterface;
using Spice.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Repository
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        ISubCategoryRepository SubCategoryRepository { get; }
        ICouponRepository CouponRepository { get; }
        IImportHistoryRepository ImportHistoryRepository { get; }
        IApplicationUserRepository ApplicationUserRepository { get; }
        INewsRepository NewsRepository { get; }
        IOrderHeaderRepository OrderHeaderRepository { get; }
        IMenuItemRepository MenuItemRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        IRatingRepository RatingRepository { get; }
        IFavoritedProductRepository FavoritedProductRepository { get; }
        Task<int> SaveChangesAsync();
        void SaveChanges();
        void Dispose();
    }
}
