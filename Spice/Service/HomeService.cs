using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Repository;
using Spice.Service.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Service
{
    public class HomeService : IHomeService
    {

        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public HomeService(IUnitOfWork unitOfWork, IUserService userService)
        {
            this._unitOfWork = unitOfWork;
            this._userService = userService;
        }

        public IndexHomeVM PrepareForHomeIndex()
        {
            IndexHomeVM IndexVM = new IndexHomeVM()
            {
                ListPopularMenuItem = _unitOfWork.MenuItemRepository.FilterMostPopularMenuItems(),
                ListNewMenuItem = _unitOfWork.MenuItemRepository.FilterMostBestSellerMenuItems(),                                                    
                ListBestSellerMenuItem = _unitOfWork.MenuItemRepository.FilterMostBestSellerMenuItems(),
                ListNews = _unitOfWork.NewsRepository.ReadAllCouponOrNews().Take(2)                                         
            };
            return IndexVM;
        }

        public void confirmOrRemoveFavoritedProduct(int itemId)
        {
            string userId = _userService.GetUserId();            
            var temp = _unitOfWork.FavoritedProductRepository.findByItemIdAndUserId(itemId, userId);
            if (temp == null)
            {
                FavoritedProduct favoritedProduct = new FavoritedProduct()
                {
                    ItemId = itemId,
                    UserId = userId
                };
                _unitOfWork.FavoritedProductRepository.Create(favoritedProduct);                
            }
            else
            {
                _unitOfWork.FavoritedProductRepository.Delete(temp);
            }
            _unitOfWork.SaveChanges();
        }
    }
}
