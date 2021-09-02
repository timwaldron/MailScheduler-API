using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Config
{
    public class AppSettings : IAppSettings
    {
        public DatabaseSettings Database { get; set; }
        public MailSettings MailSettings { get; set; }
    }
}
