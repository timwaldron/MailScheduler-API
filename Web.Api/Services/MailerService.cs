using Hangfire;
using MailScheduler.Config;
using MailScheduler.Helpers;
using MailScheduler.Models;
using MailScheduler.Models.Dtos;
using MailScheduler.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MailScheduler.Services
{
    public class MailerService : IMailerService
    {
        private readonly IAppSettings _settings;
        private readonly ISchedulerRepository _schedulerRepository;
        private readonly ITelemetryService _telemetryService;

        public MailerService(IAppSettings settings, ISchedulerRepository schedulerRepository, ITelemetryService telemetryService)
        {
            _settings = settings;
            _schedulerRepository = schedulerRepository;
            _telemetryService = telemetryService;
        }

        public async Task<string> SendMail(UserScheduleDto user, string followupDate)
        {
            string response = string.Empty;
            string entrySurveyId = "742391"; // TODO: Put this as "entrySurveyId" in the global plugin settings (stored in db)

            try
            {
                MailMessage mail = new MailMessage();

                SmtpClient smtpServer = new SmtpClient(_settings.MailSettings.SmtpServer);
                smtpServer.Port = _settings.MailSettings.Port;
                smtpServer.EnableSsl = _settings.MailSettings.EnableSsl;
                smtpServer.Credentials = new System.Net.NetworkCredential(_settings.MailSettings.Username, _settings.MailSettings.Password);

                mail.From = new MailAddress(_settings.MailSettings.FromAddress);
                mail.Subject = "Post-Operative Survey"; // TODO: Make this configurable in global plugin settings?

                mail.To.Add(user.Email);

                var surgeryType = InjuryTypeToSurgeryType(user.InjuryType);
                var timepointResponse = TimepointToString(user.SurgeryDate, followupDate);
                var timepoint = timepointResponse.Item1;
                var surgeryInterval = timepointResponse.Item2;

                // Build URL from user data
                var surveyURL = _settings.MailSettings.BaseSurveyUrl
                    .Replace("{SID}", entrySurveyId)
                    .Replace("{FIRSTNAME}", user.FirstName)
                    .Replace("{LASTNAME}", user.LastName)
                    .Replace("{EMAIL}", user.Email)
                    .Replace("{TOKEN}", user.Token)
                    .Replace("{INJURYTYPE}", user.InjuryType)
                    .Replace("{SURGERYDATE}", user.SurgeryDate)
                    .Replace("{SIDE}", user.InjurySide)
                    .Replace("{SURVEYINTERVAL}", surgeryInterval.ToString());

                //Replace any fields in the email html template
                mail.IsBodyHtml = true;
                mail.Body = Templates.EmailHTML
                    .Replace("{FIRSTNAME}", user.FirstName)
                    .Replace("{LASTNAME}", user.LastName)
                    .Replace("{TIMEPOINT}", timepoint)
                    .Replace("{SURGERYTYPE}", surgeryType)
                    .Replace("{SURVEYURL}", surveyURL);

                await smtpServer.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                await _telemetryService.Log($"[{DateTime.Now.ToLongTimeString()}] Failure sending mail for {user.Id} @ {user.Token} / {user.SurveyId}: {ex.Message}", Severity.Error);
                response = ex.ToString();
            }

            return response;
        }

        private string InjuryTypeToSurgeryType(string injuryType)
        {
            // This text is just temporary, need to replace
            return injuryType switch
            {
                "H" => "hip",
                var injury when injury == "KN" || injury == "KA" => "knee",
                var injury when injury == "SA" || injury == "SI" => "shoulder",
                _ => "(Unknown Surgery Type, please contact us)",
            };
        }

        // TODO: Refactor this logic so it isn't static
        private (string, int) TimepointToString(string surgery, string followup)
        {
            var surgeryDate = DateTime.Parse(surgery);
            var followupDate = DateTime.Parse(followup);
            var difference = (followupDate - surgeryDate).TotalDays - 2; // Minus 2 days just to make sure it's within the followup bracket

            switch (difference)
            {
                case <= 42: return ("six weeks", 42);
                case <= 90: return ("three months", 90);
                case <= 180: return ("six months", 180);
                case <= 365: return ("twelve months", 365);
                case <= 730: return ("two years", 730);
                case <= 1825: return ("five years", 1825);
                case <= 3650: return ("ten years", 3650);
                default: return ("(Unknown Timepoint, please contact us)", -1);
            }
        }

        public async Task AssessAndSendMail()
        {
            await _telemetryService.Log($"[{DateTime.Now.ToLongTimeString()}] Assessing and sending mail...");

            try
            {
                var allSchedules = await _schedulerRepository.GetAllSchedules();
                var date = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

                // Iterate every schedule
                foreach (var schedule in allSchedules)
                {
                    foreach (var followupDate in schedule.FollowupDates)
                    {
                        if (followupDate == date)
                        {
                            await _telemetryService.Log($"[{DateTime.Now.ToLongTimeString()}] Sending mail {schedule.Id} @ {schedule.Token} / {schedule.SurveyId}");
                            await SendMail(schedule, followupDate);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _telemetryService.Log($"[{DateTime.Now.ToLongTimeString()}] Failure within AssessAndSendMail: {ex.Message}", Severity.Error);
            }
        }
    }
}
