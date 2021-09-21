using MailScheduler.Models.Dtos;
using MailScheduler.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Services
{
    public class TelemetryService : ITelemetryService
    {
        private readonly ITelemetryRepository _repository;

        public TelemetryService(ITelemetryRepository repository)
        {
            _repository = repository;
        }

        // TODO: Refactor all this logic, shouldn't assume successfulness
        public async Task<string> Log(string message, Severity severity = Severity.Information, string task = "")
        {
            var successful = severity == Severity.Information || severity == Severity.Warning;

            var log = new TelemetryDto()
            {
                Message = message,
                Severity = severity,
                Successful = successful,
                Timestamp = DateTime.Now
            };

            var id = await _repository.Log(log);

            return id;
        }

        public async Task<string> Log(TelemetryDto dto)
        {
            dto.Timestamp = DateTime.Now;
            var id = await _repository.Log(dto);

            return id;
        }

        public async Task<List<string>> GetData()
        {
            var telemetryData = (await _repository.GetData())?.OrderByDescending(r => r.Timestamp);

            var results = new List<string>();

            foreach (var data in telemetryData)
            {
                var severity = Enum.GetName(typeof(Severity), data.Severity);
                results.Add($"[{data.Timestamp}] - {severity}: {data.Message}");
            }

            return results;
        }
    }
}
