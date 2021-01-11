using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Service.ServiceInterfaces
{
    public interface ISessionService
    {
        void ClearCart();
        void ClearCoupon();
        object GetSession(string code);
    }
}
