using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.Builder
{
    public class OrderDetailBuilder:IOrderDetailBuilder
    {
        private int Count;
        private string Name;
        private string Description;
        private double Price;
        private int OrderId;
        private int MenuItemId;

        public IOrderDetailBuilder AddQuantity(int Quantity)
        {
            this.Count = Quantity;
            return this;
        }
        public IOrderDetailBuilder AddName(string Name)
        {
            this.Name = Name;
            return this;
        }
        public IOrderDetailBuilder MoreDescription(string Description)
        {
            this.Description = Description;
            return this;
        }
        public IOrderDetailBuilder CalPrice(double Price)
        {
            this.Price = Price;
            return this;
        }
        public IOrderDetailBuilder LinkOrderID(int OrderID)
        {
            this.OrderId = OrderID;
            return this;
        }
        public IOrderDetailBuilder LinkMenuItemID(int MenuItemID)
        {
            this.MenuItemId = MenuItemID;
            return this;
        }

        public OrderDetails Build()
        {
            return new OrderDetails(this.Count, this.Name, this.Description, this.Price, this.OrderId, this.MenuItemId);
        }
    }
}
