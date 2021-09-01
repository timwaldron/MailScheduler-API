using MailScheduler.Models;
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

        public string InitUserSchedule(UserScheduleDto dto)
        {
            // Filter lookup (Email && Token && SurveyId)
            // If user exists, return (do nothing)
            var user = _repository.GetScheduleByToken(dto.SurveyId, dto.Token);

            // Returning user, no need to create initial db entry
            if (user != null)
            {
                return string.Empty;
            }

            dto.RecruitmentDate = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            // If a surgery date is supplied, calculate the followup dates
            if (!string.IsNullOrEmpty(dto.SurgeryDate))
            {
                var surgeryDate = DateTime.Parse(dto.SurgeryDate);
                dto.FollowupDates = GenerateFollowupDates(surgeryDate);

                // Date string that's coming in is formatted: dd.MM.yyyy, this reformats it to our style
                dto.SurgeryDate = surgeryDate.ToString(Constants.DATETIME_FORMAT);
            }

            return _repository.SaveUserSchedule(dto);
        }

        public List<UserScheduleDto> GetAllSchedules()
        {
            return _repository.GetAllSchedules();
        }

        // Static dates for now, possibly expand in the future to be set via LimeSurvey?
        private List<string> GenerateFollowupDates(DateTime surgeryDate)
        {
            var timelineDays = new int[] {
                42,   // 6 Weeks
                90,   // 3 Months
                180,  // 6 Months
                365,  // 12 Months
                730,  // 2 Years
                1825, // 5 Years
                3650, // 10 Years
            };

            var followups = new List<string>();
            foreach (var day in timelineDays)
            {
                var date = surgeryDate.AddDays(day).ToString(Constants.DATETIME_FORMAT);
                followups.Add(date);
            }

            return followups;
        }
    }
}
