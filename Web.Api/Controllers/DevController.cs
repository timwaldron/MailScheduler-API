using MailScheduler.Models.Dtos;
using MailScheduler.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class DevController : ControllerBase
    {
        private readonly IMailerService _service;

        public DevController(IMailerService service)
        {
            _service = service;
        }

        [HttpGet("mailtest")]
        public async Task<IActionResult> MailTest()
        {
            var user = new UserScheduleDto()
            {
                FirstName = "Timothy",
                LastName = "Waldron",
                Email = "tim@waldron.im",
                SurveyId = "377889",
                Token = "luHLDn4SCCMvQaQ",

            };
            user.SurveyURL = $"http://localhost/limesurvey/index.php/{user.SurveyId}?token={user.Token}&newtest=Y";

            var mailResponse = await _service.SendMail(user);

            return Ok(mailResponse);
        }
    }
}
