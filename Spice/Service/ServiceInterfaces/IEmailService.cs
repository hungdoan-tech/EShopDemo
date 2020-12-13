using Spice.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Service.ServiceInterfaces
{
    interface IEmailService
    {
        public void SendMailSummitted(OrderDetailsCart detailCart, Claim claim);
    }
}
