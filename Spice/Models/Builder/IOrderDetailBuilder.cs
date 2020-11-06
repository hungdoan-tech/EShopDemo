using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.Builder
{
    public interface IOrderDetailBuilder
    {
        IOrderDetailBuilder AddQuantity(int Quantity);
        IOrderDetailBuilder AddName(string Name);
        IOrderDetailBuilder MoreDescription(string Description);
        IOrderDetailBuilder CalPrice(double Price);
        IOrderDetailBuilder LinkOrderID(int OrderID);
        IOrderDetailBuilder LinkMenuItemID(int MenuItemID);
        OrderDetails Build();
    }
}
