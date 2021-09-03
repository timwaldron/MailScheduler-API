using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Config
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string ServerUrl { get; set; }
        public string Name { get; set; }
        public string AdminDb { get; set; }
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }
    }
}
