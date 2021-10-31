using MoneyHeist.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyHeist.MailHelperClasses
{
    public class MailLogic
    {
        private readonly IMailService mailService;
        public MailLogic(IMailService mailService)
        {
            this.mailService = mailService;
        }

        public async void SendMail(MailRequest request, int notificationType)
        {
            try
            {
                await mailService.SendEmailAsync(request, notificationType);
            }
            catch (Exception ex)
            {
                //would add log to db that mail failed to send
                //so code can continue and not throw
            }

        }
    }
}
