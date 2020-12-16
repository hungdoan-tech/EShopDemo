using Spice.Models;
using System.Collections.Generic;

namespace Spice.RepositoryInterface
{
    public interface INewsRepository: IRepository<News>
    {
        News ReadOneNewsIncludeMenuItem(int? id);
        IEnumerable<News> ReadAllNewsIncludeMenuItem();
        IEnumerable<News> ReadAllCouponOrNews();
    }
}
