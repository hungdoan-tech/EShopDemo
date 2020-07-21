using Spice.Data;
using Spice.Models;
using Spice.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Repository
{
    public class ImportHistoryRepository :  GenericRepository<ImportHistory>, IImportHistoryRepository
    {
        public ImportHistoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
