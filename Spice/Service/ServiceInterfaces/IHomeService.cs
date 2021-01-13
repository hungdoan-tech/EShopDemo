using Spice.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Service.ServiceInterfaces
{
    public interface IHomeService
    {
        IndexHomeVM PrepareForHomeIndex();
        public void confirmOrRemoveFavoritedProduct(int id);
    }
}
