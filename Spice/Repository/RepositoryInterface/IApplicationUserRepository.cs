using Spice.Models;

namespace Spice.RepositoryInterface
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser> 
    {
        public ApplicationUser ReadOneByStringID(string id);
    }
}
