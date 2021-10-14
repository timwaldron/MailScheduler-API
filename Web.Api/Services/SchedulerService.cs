using MailScheduler.Helpers;
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
        private readonly IMailerService _mailerService;
        private readonly ITelemetryService _telemetryService;

        public SchedulerService(ISchedulerRepository repository, IMailerService mailerService, ITelemetryService telemetryService)
        {
            _repository = repository;
            _mailerService = mailerService;
            _telemetryService = telemetryService;
        }

        public async Task<UserScheduleDto> GetScheduleByToken(string surveyId, string token)
        {
            return await _repository.GetScheduleByToken(surveyId, token);
        }

        public async Task<string> SaveUserSchedule(UserScheduleDto dto)
        {
            // Check if current user exists by surveyId/token
            var existingUser = await GetScheduleByToken(dto.SurveyId, dto.Token);
            if (existingUser != null)
            {
                dto.Id = existingUser.Id;
            }

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

            return await _repository.SaveUserSchedule(dto);
        }

        public async Task<string> InitUserSchedule(UserScheduleDto dto)
        {
            // Filter lookup (Email && Token && SurveyId)
            // If user exists, return (do nothing)
            var user = await _repository.GetScheduleByToken(dto.SurveyId, dto.Token);

            // Returning user, no need to create initial db entry
            if (user != null)
            {
                return string.Empty;
            }

            dto.RecruitmentDate = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

            // If a surgery date is supplied, calculate the followup dates
            if (!string.IsNullOrEmpty(dto.SurgeryDate))
            {
                var surgeryDate = DateTime.ParseExact(dto.SurgeryDate, "dd.MM.yyyy", null);
                dto.FollowupDates = GenerateFollowupDates(surgeryDate);

                // Date string that's coming in is formatted: dd.MM.yyyy, this reformats it to our style
                dto.SurgeryDate = surgeryDate.ToString(Constants.DATETIME_FORMAT);
            }

            return await _repository.SaveUserSchedule(dto);
        }

        public async Task<List<UserScheduleDto>> GetAllSchedules()
        {
            return await _repository.GetAllSchedules();
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

        public async Task DebugMailTest(string followupDate, string token, string surveyId)
        {
            var schedule = await GetScheduleByToken(surveyId, token);

            await _mailerService.SendMail(schedule, followupDate);
        }

        public async Task CleanupGhostUsers(List<UserScheduleDto> users)
        {
            var userTokens = users.Select(u => u.Token);
            await _telemetryService.Log("Running cleanup on ghost users...");

            var dbUsers = await _repository.GetAllSchedules();
            var ghostUserTokens = new List<string>();

            foreach (var user in dbUsers)
            {
                if (!userTokens.Contains(user.Token))
                {
                    ghostUserTokens.Add(user.Token);
                    await _telemetryService.Log($"Removing ghost user {user.FirstName} {user.LastName} ({user.Email}/{user.Token})");
                }
            }

            if (ghostUserTokens.Any())
            {
                await _repository.RemoveByToken(ghostUserTokens);
            }
        }
    }
}
