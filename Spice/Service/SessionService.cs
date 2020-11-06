using Microsoft.AspNetCore.Http;
using Spice.Data;
using Spice.Extensions;
using Spice.Models.ViewModels;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Service
{
    public class SessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public void Clear()
        {
            var lstShoppingCart = new List<MenuItemsAndQuantity>();
            _httpContextAccessor.HttpContext.Session.Set(SD.ssShoppingCart, lstShoppingCart);
        }
    }
}
