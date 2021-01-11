using Microsoft.AspNetCore.Http;
using Spice.Extensions;
using Spice.Models.ViewModels;
using Spice.Service.ServiceInterfaces;
using Spice.Utility;
using System.Collections.Generic;


namespace Spice.Service
{
    public class SessionService: ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public void ClearCart()
        {
            var lstShoppingCart = new List<MenuItemsAndQuantity>();
            _httpContextAccessor.HttpContext.Session.Set(SD.ssShoppingCart, lstShoppingCart);
        }

        public void ClearCoupon()
        {
            _httpContextAccessor.HttpContext.Session.Get<List<MenuItemsAndQuantity>>(SD.ssShoppingCart).Clear();
        }

        public object GetSession(string code)
        {
            return _httpContextAccessor.HttpContext.Session.Get(code);
        }
        public List<MenuItemsAndQuantity> GetSessionListQuantity()
        {
            return _httpContextAccessor.HttpContext.Session.Get<List<MenuItemsAndQuantity>>(SD.ssShoppingCart);
        }
    }
}
