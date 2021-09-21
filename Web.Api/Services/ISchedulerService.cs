using MailScheduler.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Services
{
    public interface ISchedulerService
    {
        Task<UserScheduleDto> GetScheduleByToken(string surveyId, string token);

        // TODO: Return ServiceResult? (If failed/successful / if failed then why?)
        Task<string> SaveUserSchedule(UserScheduleDto dto);
        Task<string> InitUserSchedule(UserScheduleDto dto);
        Task<List<UserScheduleDto>> GetAllSchedules();
        Task DebugMailTest(string followupDate, string token, string surveyId);
    }
}
