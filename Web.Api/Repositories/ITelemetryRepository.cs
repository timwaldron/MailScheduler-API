using MailScheduler.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Repositories
{
    public interface ITelemetryRepository
    {
        Task<string> Log(TelemetryDto dto);
        Task<List<TelemetryDto>> GetData();
    }
}
