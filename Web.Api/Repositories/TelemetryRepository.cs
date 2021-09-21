using MailScheduler.Config;
using MailScheduler.Mappers;
using MailScheduler.Models.Dtos;
using MailScheduler.Models.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Repositories
{
    public class TelemetryRepository : RepositoryBase<Telemetry>, ITelemetryRepository
    {
        public override string CollectionName { get => "telemetry"; }

        public TelemetryRepository(IAppSettings settings) : base(settings)
        {
        }

        public async Task<string> Log(TelemetryDto dto)
        {
            var entity = dto.ToEntity();

            var obj = await Upsert(entity);

            return obj?.Id;
        }

        public async Task<List<TelemetryDto>> GetData()
        {
            var entity = await base.GetAll();

            return entity.Select(e => e.ToDto()).ToList();
        }
    }
}
