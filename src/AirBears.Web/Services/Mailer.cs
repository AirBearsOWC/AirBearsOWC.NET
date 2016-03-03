using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace AirBears.Web.Services
{
    public class Mailer : IMailer
    {
        public async Task SendAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();

            message.To.Add(new MailboxAddress(string.Empty, to));
            message.Subject = subject;
            message.Body = new TextPart() { Text = body };

            await Send(message);
        }

        private async Task Send(MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                //smtp.Host = "smtp.gmail.com";
                //smtp.Port = 587; // Gmail can use ports 25, 465 & 587; but must be 25 for medium trust environment.
                //smtp.EnableSsl = true;
                //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //smtp.UseDefaultCredentials = false;
                //client.Credentials = new NetworkCredential("airbears.uav@gmail.com", "mickey5356t6*");

                await client.AuthenticateAsync("airbears.uav@gmail.com", "mickey5356t6*");

                message.From.Add(new MailboxAddress("Air Bears", "airbears.uav@leaguestone.com"));
                message.Bcc.Add(new MailboxAddress("Air Bears", "airbears.uav@gmail.com"));
                //message.IsBodyHtml = true;

                //message.Body = message.Body.Replace("\n", "<br />");

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                //await smtp.SendMailAsync(message);
            }
        }
    }
}
