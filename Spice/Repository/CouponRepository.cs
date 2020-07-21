using Spice.Data;
using Spice.Models;
using Spice.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Repository
{
    public class CouponRepository : GenericRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Coupon FirstMatchName(string code)
        {
            return this.dbSet.Where(c => c.Name.ToLower() == code).FirstOrDefault();
        }
    }
}
