using MailScheduler.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Services
{
    public interface IMailerService
    {
        string SendMail(UserScheduleDto user, string timepoint);
        void AssessAndSendMail();
    }
}
