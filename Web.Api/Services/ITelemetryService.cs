using MailScheduler.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Services
{
    public interface ITelemetryService
    {
        Task<string> Log(string message, Severity severity = Severity.Information, string task = "");
        Task<string> Log(TelemetryDto dto);
        Task<List<string>> GetData();
    }
}
