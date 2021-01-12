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
        private readonly IUnitOfWork _unitOfWork;

        public HomeService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
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
    }
}
