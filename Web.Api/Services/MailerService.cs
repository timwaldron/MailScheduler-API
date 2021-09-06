using Hangfire;
using MailScheduler.Config;
using MailScheduler.Models;
using MailScheduler.Models.Dtos;
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
        private readonly ISchedulerService _service;

        public MailerService(IAppSettings settings, ISchedulerService service)
        {
            _settings = settings;
            _service = service;
        }

        public string SendMail(UserScheduleDto user)
        {
            string response = string.Empty;

            try
            {
                MailMessage mail = new MailMessage();

                SmtpClient smtpServer = new SmtpClient(_settings.MailSettings.SmtpServer);
                smtpServer.Port = _settings.MailSettings.Port;
                smtpServer.EnableSsl = _settings.MailSettings.EnableSsl;
                smtpServer.Credentials = new System.Net.NetworkCredential(_settings.MailSettings.Username, _settings.MailSettings.Password);

                mail.From = new MailAddress(_settings.MailSettings.FromAddress);
                mail.Subject = "Novar Survey";

                // Build URL from user data
                var surveyURL = _settings.MailSettings.BaseSurveyUrl
                    .Replace("{SID}", "742391") // TODO: Assess if we can hardcode this
                    .Replace("{FIRSTNAME}", user.FirstName)
                    .Replace("{LASTNAME}", user.LastName)
                    .Replace("{EMAIL}", user.Email)
                    .Replace("{TOKEN}", user.Token)
                    .Replace("{INJURYTYPE}", user.InjuryType);

                mail.To.Add(user.Email);
                string body = @"
Dear {NAME},

To participate, please click on the link below.

Survey Link: 
{SURVEYURL}

If you need any assistance with this survey or if you have any enquiries in regard to the same, please do not hesitate to contact us.

You can call us on [need phone number] or email us at [need email address] 

Your next scheduled email is on: X (TO BE FILLED IN)

With Best Regards

The team at Novar Specialist Healthcare
";

                mail.Body = body
                    .Replace("{NAME}", $"{user.FirstName} {user.LastName}")
                    .Replace("{SURVEYURL}", $"{surveyURL}");

                smtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                response = ex.ToString();
            }

            return response;
        }

        public void AssessAndSendMail()
        {
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Assessing and sending mail...");

            try
            {
                var allSchedules = _service.GetAllSchedules();
                var date = DateTime.Now.ToString(Constants.DATETIME_FORMAT);

                // Iterate every schedule
                foreach (var schedule in allSchedules)
                {

                    foreach (var followupDate in schedule.FollowupDates)
                    {
                        if (followupDate == date)
                        {
                            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Sending mail: {schedule.FirstName} {schedule.LastName} ({schedule.Email}) for date {followupDate}.");
                            SendMail(schedule);
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // TODO: HANDLE / IMPLEMENT THIS CORRECTLY
            }
        }
    }
}
