using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.RepositoryInterface
{
    public interface ICouponRepository: IRepository<Coupon>
    {
        Coupon FirstMatchName(string code);
    }
}
