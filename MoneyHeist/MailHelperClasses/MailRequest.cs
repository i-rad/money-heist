using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyHeist.MailHelperClasses
{
    public class MailRequest
    {
        public string MemberName { get; set; }
        public string HeistName { get; set; }
        public string ToEmail { get; set; }
    }
}
