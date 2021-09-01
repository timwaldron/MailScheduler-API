using MailScheduler.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Repositories
{
    public interface ISchedulerRepository
    {
        UserScheduleDto GetScheduleByToken(string surveyId, string token);
        string SaveUserSchedule(UserScheduleDto dto);
        List<UserScheduleDto> GetAllSchedules();
    }
}
