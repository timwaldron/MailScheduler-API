using MailScheduler.Models.Dtos;
using MailScheduler.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Services
{
    public class SchedulerService : ISchedulerService
    {
        private readonly ISchedulerRepository _repository;

        public SchedulerService(ISchedulerRepository repository)
        {
            _repository = repository;
        }

        public UserScheduleDto GetScheduleByToken(string surveyId, string token)
        {
            return _repository.GetScheduleByToken(surveyId, token);
        }

        public string SaveUserSchedule(UserScheduleDto dto)
        {
            // Recruitment date is required, should be set automatically when adding a participant.
            var recruitmentDate = DateTime.Parse(dto.RecruitmentDate);

            if (recruitmentDate.Year < DateTime.Now.Year)
            {
                // TODO: Handle this exception
                throw new Exception();
            }

            if (!string.IsNullOrEmpty(dto.SurgeryDate) && dto.RecalcFollowupDates == "true")
            {
                var surgeryDate = DateTime.Parse(dto.SurgeryDate);
                dto.FollowupDates = GenerateFollowupDates(surgeryDate);
            }

            return _repository.SaveUserSchedule(dto);
        }

        // Static dates for now, possibly expand in the future to be set via LimeSurvey?
        private List<string> GenerateFollowupDates(DateTime surgeryDate)
        {
            // Formatting DateTime.Now.ToString : yyyy-MM-dd
            var followups = new List<string>();

            followups.Add(surgeryDate.AddDays(42).ToString("yyyy-MM-dd")); // 6 Weeks
            followups.Add(surgeryDate.AddDays(90).ToString("yyyy-MM-dd")); // 3 Months
            followups.Add(surgeryDate.AddDays(180).ToString("yyyy-MM-dd")); // 6 Months
            followups.Add(surgeryDate.AddDays(365).ToString("yyyy-MM-dd")); // 12 Months
            followups.Add(surgeryDate.AddDays(730).ToString("yyyy-MM-dd")); // 2 Years
            followups.Add(surgeryDate.AddDays(1825).ToString("yyyy-MM-dd")); // 5 Years
            followups.Add(surgeryDate.AddDays(3650).ToString("yyyy-MM-dd")); // 10 Years

            return followups;
        }
    }
}
