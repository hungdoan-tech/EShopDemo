using Spice.Data;
using Spice.Models;
using Spice.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Repository
{
    public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository
    {
        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public ApplicationUser ReadByID(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException($"Key value must not be null");
            }
            try
            {
                return this.dbSet.Find(id);
            }
            catch (Exception)
            {
                throw new Exception("Couldn't retrieve entities");
            }
        }
    }
}
