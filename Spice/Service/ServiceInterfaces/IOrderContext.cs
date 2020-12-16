using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Service.ServiceInterfaces
{
    public interface IOrderContext
    {
        public void SetState(IOrderState state);

        public void ApplyState(int OrderId);        
    }
}
