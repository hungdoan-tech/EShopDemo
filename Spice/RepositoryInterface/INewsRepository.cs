using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.RepositoryInterface
{
    public interface INewsRepository: IRepository<News>
    {
        News ReadOneNewsIncludeMenuItem(int? id);
        IEnumerable<News> ReadAllNewsIncludeMenuItem();
        IEnumerable<News> ReadAllCouponOrNews();
    }
}
