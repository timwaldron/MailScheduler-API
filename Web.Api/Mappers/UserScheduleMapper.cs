using AutoMapper;
using MailScheduler.Models.Dtos;
using MailScheduler.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Mappers
{
    public static class UserScheduleMapper
    {
        private static IMapper mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<UserScheduleMapperProfile>()));

        public static UserScheduleDto ToDto(this UserSchedule entity)
        {
            return mapper.Map<UserScheduleDto>(entity);
        }

        public static UserSchedule ToEntity(this UserScheduleDto dto)
        {
            return mapper.Map<UserSchedule>(dto);
        }
    }

    public class UserScheduleMapperProfile : Profile
    {
        public UserScheduleMapperProfile()
        {
            CreateMap<UserSchedule, UserScheduleDto>()
                .ForMember(x => x.RecalcFollowupDates, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
