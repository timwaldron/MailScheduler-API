using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Config
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string ConnectionUrl { get; set; }
        public string Name { get; set; }
    }
}
