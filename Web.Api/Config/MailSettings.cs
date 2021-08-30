using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Config
{
    public class MailSettings : IMailSettings
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string BaseSurveyURL { get; set; }
        public string FromAddress { get; set; }
    }
}
