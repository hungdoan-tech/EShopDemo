using Microsoft.AspNetCore.Identity.UI.Services;
using Spice.Models;
using Spice.Repository;
using Spice.Service.ServiceInterfaces;
using Spice.Utility;
using System.Linq;


namespace Spice.Service.State
{
    public class CancelledState : IOrderState
    {
        public void HandleRequest(IUnitOfWork _unitOfWork, IEmailSender _emailSender, int OrderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.ReadOne(OrderId);
            orderHeader.Status = SD.StatusCancelled;

            try
            {
                var ListOrderDetail = _unitOfWork.OrderDetailRepository.Get(a => a.OrderId == orderHeader.Id).ToList();
                foreach(var orderDetail in ListOrderDetail)
                {
                    _unitOfWork.MenuItemRepository.ReadOne(orderDetail.MenuItemId).Quantity += orderDetail.Count; 
                }
                _unitOfWork.OrderDetailRepository.DeleteRange(ListOrderDetail);
                _unitOfWork.OrderHeaderRepository.Delete(orderHeader);
                _unitOfWork.SaveChanges();
            }
            catch
            {
                _unitOfWork.Dispose();
            }
            _emailSender.SendEmailAsync(_unitOfWork.ApplicationUserRepository.ReadOneByStringID(orderHeader.UserId).Email, "Order number " + orderHeader.Id.ToString() + " has been cancelled", "Order "+ orderHeader.Id.ToString() + "has been cancelled.");
        }
    }
}
