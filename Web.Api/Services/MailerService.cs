using MailScheduler.Config;
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

        public MailerService(IAppSettings settings)
        {
            _settings = settings;
        }

        public async Task<string> SendMail(UserScheduleDto user)
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

                var surveyURL = _settings.MailSettings.BaseSurveyURL.Replace("{SID}", user.SurveyId);

                mail.To.Add(user.Email);
                string body = @"
Dear {NAME},

To participate, please click on the link below.

Survey Link: 
{SURVEYURL}

If you need any assistance with this survey or if you have any enquiries in regard to the same, please do not hesitate to contact us.

You can call us on [need phone number] or email us at [need email address] 

Your next scheduled email is on: X

With Best Regards

The team at Novar Specialist Healthcare
";

                mail.Body = body
                    .Replace("{NAME}", $"{user.FirstName} {user.LastName}")
                    .Replace("{SURVEYURL}", $"{surveyURL}");

                await smtpServer.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                response = ex.ToString();
            }

            return response;
        }
    }
}
