using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Models.Dtos
{
    public class TelemetryDto
    {
        public string Id { get; set; }
        public Severity Severity { get; set; }
        public string Message { get; set; }
        public bool Successful { get; set; }
        public DateTime Timestamp { get; set; }
    }

    // TODO: Move this into another location
    public enum Severity
    {
        Information,
        Warning,
        Error,
        Critical
    }
}
