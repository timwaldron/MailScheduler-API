using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Config
{
    public class AppSettings : IAppSettings
    {
        public string MongoConnectionString { get; set; }
        public string MongoDatabaseName { get; set; }
        public MailSettings MailSettings { get; set; }
    }
}
