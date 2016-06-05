using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using AirBears.Web.Settings;
using Microsoft.Extensions.OptionsModel;

namespace AirBears.Web.Services
{
    public class Mailer : IMailer
    {
        private SmtpSettings SmtpSettings { get; set; }
        private AppSettings AppSettings { get; set; }

        public Mailer(IOptions<SmtpSettings> smtpSettings, IOptions<AppSettings> appSettings)
        {
            SmtpSettings = smtpSettings.Value;
            AppSettings = appSettings.Value;
        }

        public async Task SendAsync(string to, string subject, string body, bool isHtml = false)
        {
            var message = new MimeMessage();
            var subtype = isHtml ? "html" : "plain";

            message.To.Add(new MailboxAddress(string.Empty, to));
            message.Subject = subject;
            message.Body = new TextPart(subtype) { Text = body };

            await Send(message);
        }

        private async Task Send(MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync(SmtpSettings.Username, SmtpSettings.Password);

                message.From.Add(new MailboxAddress("Air Bears Team", "teamcentral@airbears.org"));
                message.Bcc.Add(new MailboxAddress(string.Empty, AppSettings.PrimaryAppRecipient));

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
