using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyHeist.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailHelperClasses.MailRequest mailRequest, int notificationType);
    }
}
