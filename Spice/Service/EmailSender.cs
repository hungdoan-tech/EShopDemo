using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Spice.Service
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com")
            {
                UseDefaultCredentials = true,
                EnableSsl = true,
                Credentials = new NetworkCredential("hungdoan426@gmail.com", "H_7hung426")
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress("account-security-noreply@yourdomain.com")
            };
            mailMessage.To.Add(email);
            mailMessage.Subject = subject;
            mailMessage.Body = htmlMessage;
            try
            {
                return client.SendMailAsync(mailMessage);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
    }
}