using MailScheduler.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Services
{
    public interface ISchedulerService
    {
        UserScheduleDto GetScheduleByToken(string surveyId, string token);

        // TODO: Return ServiceResult? (If failed/successful / if failed then why?)
        string SaveUserSchedule(UserScheduleDto dto);
        string InitUserSchedule(UserScheduleDto dto);
        List<UserScheduleDto> GetAllSchedules();
    }
}
