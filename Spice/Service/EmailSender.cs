using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Service
{
    public class EmailSender : IEmailSender
    {
        public EmailOptions Options { get; set; }

        public EmailSender(IOptions<EmailOptions> emailOptions)
        {
            Options = emailOptions.Value;
        }

        //public Task SendEmailAsync(string email, string subject, string message)
        //{
        //    return Execute(Options.SendGridKey, subject, message, email);
        //}

        //public Task SendEmailAsync(string email, string subject, string message)
        //{
        //    return Execute(Options.SendGridKey, subject, message, email);
        //}

        //private Task Execute(string sendGridKey, string subject, string message, string email)
        //{
        //    var client = new SendGridClient(sendGridKey);
        //    var msg = new SendGridMessage()
        //    {
        //        From = new EmailAddress("admin@spice.com", "Spice Restaurant"),
        //        Subject = subject,
        //        PlainTextContent = message,
        //        HtmlContent = message
        //    };
        //    msg.AddTo(new EmailAddress(email));
        //    try
        //    {
        //        return client.SendEmailAsync(msg);
        //    }
        //    catch(Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //    return null;
        //}

        //public Task SendEmailAsync(List<string> emails, string subject, string message)
        //{

        //    return Execute(Environment.GetEnvironmentVariable("SENDEMAILDEMO_ENVIRONMENT_SENDGRID_KEY"), subject, message, emails);
        //}

        public Task Execute(string apiKey, string subject, string message, List<string> emails)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("hungdoan426@domain.com", "Bekenty Jean Baptiste"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };

            foreach (var email in emails)
            {
                msg.AddTo(new EmailAddress(email));
            }

            Task response = client.SendEmailAsync(msg);
            return response;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emails = new List<string>();
            emails.Add(email);
            return Execute(Environment.GetEnvironmentVariable("SENDEMAILDEMO_ENVIRONMENT_SENDGRID_KEY"), subject, htmlMessage, emails);
        }
    }
}
