﻿using Spice.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Service.ServiceInterfaces
{
    public interface ISessionService
    {
        void Set<T>(string key, T value);
        void ClearCart();
        void ClearCoupon();
        object GetSession(string code);
        List<MenuItemsAndQuantity> GetSessionListQuantity();
    }
}
