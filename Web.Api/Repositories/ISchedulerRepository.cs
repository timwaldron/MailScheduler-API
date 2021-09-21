using MailScheduler.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Repositories
{
    public interface ISchedulerRepository
    {
        Task<UserScheduleDto> GetScheduleByToken(string surveyId, string token);
        Task<string> SaveUserSchedule(UserScheduleDto dto);
        Task<List<UserScheduleDto>> GetAllSchedules();
    }
}
