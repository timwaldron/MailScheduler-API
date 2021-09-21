using AutoMapper;
using MailScheduler.Models.Dtos;
using MailScheduler.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Mappers
{
    public static class TelemetryMapper
    {
        private static IMapper mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<TelemetryMapperProfile>()));

        public static TelemetryDto ToDto(this Telemetry entity)
        {
            return mapper.Map<TelemetryDto>(entity);
        }

        public static Telemetry ToEntity(this TelemetryDto dto)
        {
            return mapper.Map<Telemetry>(dto);
        }
    }

    public class TelemetryMapperProfile : Profile
    {
        public TelemetryMapperProfile()
        {
            CreateMap<Telemetry, TelemetryDto>()
                .ReverseMap();
        }
    }
}
