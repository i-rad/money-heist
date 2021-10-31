using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MoneyHeist.MailHelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using System.IO;

namespace MoneyHeist.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequest request, int notificationType)
        {
            // for simplification added notification type
            // 1 - Added as member
            // 2 - Confirmed for heist
            // 3 - Heist started
            // 4 - Heist ended
            string MailText = "";
            if (notificationType == 1)
            {
                MailText = "Hello [username], <br /><br />You have been added to the heist pool." +
                    "<br />You will receive an email after you have been confirmed as heist member.<br /><br />Heist Team";
                MailText = MailText.Replace("[username]", request.MemberName);
            }
            else if (notificationType == 2)
            {
                MailText = "Hello [username], <br /><br />You have been confirmed as a crew member for the following heist:[HeistName]" +
                    "<br />You will receive an email when the heist starts.<br /><br />Heist Team";
                MailText = MailText.Replace("[username]", request.MemberName).Replace("[HeistName]", request.HeistName);
            }
            else if (notificationType == 3)
            {
                MailText = "Hello [username], <br /><br />The heist: [HeistName] has just started." +
                    "<br />You will receive an email when the heist ends.<br /><br />Heist Team";
                MailText = MailText.Replace("[username]", request.MemberName).Replace("[HeistName]", request.HeistName);
            }
            else
            {
                MailText = "Hello [username], <br /><br />The heist: [HeistName] has ended." +
                    "<br /><br />Heist Team";
                MailText = MailText.Replace("[username]", request.MemberName).Replace("[HeistName]", request.HeistName);
            }

            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(request.ToEmail));
            email.Subject = "Welcome to the crew";

            var builder = new BodyBuilder();
            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.DisplayName, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
