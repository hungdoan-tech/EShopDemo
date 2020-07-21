using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.RepositoryInterface
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser> 
    {
        public ApplicationUser ReadByID(string id);
    }
}
