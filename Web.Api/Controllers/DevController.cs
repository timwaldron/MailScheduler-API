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

        [HttpPost("mailtest")]
        public IActionResult MailTest([FromBody] UserScheduleDto user)
        {
            var mailResponse = _service.SendMail(user);

            return Ok(mailResponse);
        }
    }
}
