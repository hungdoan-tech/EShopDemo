using Spice.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Service.ServiceInterfaces
{
    public interface IEmailService
    {
        public void SendSummittedMail(OrderDetailsCart detailCart, Claim claim);
        public void SendNotifyEmail(int OrderId, string OrderStatus, string Message);
        public void SendCanceledMail(int OrderId);
    }
}
