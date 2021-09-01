using MailScheduler.Models.Dtos;
using MailScheduler.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SchedulerController : ControllerBase
    {
        private readonly ILogger<SchedulerController> _logger;
        private readonly ISchedulerService _service;

        public SchedulerController(
            ILogger<SchedulerController> logger,
            ISchedulerService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost]
        public IActionResult SaveUserSchedule([FromBody] UserScheduleDto dto)
        {
            var id = _service.SaveUserSchedule(dto);

            return Ok(id);
        }

        [HttpPost("init")]
        public IActionResult InitUserSchedule([FromBody] UserScheduleDto dto)
        {
            var id = _service.InitUserSchedule(dto);
            
            // TODO: Assess if we need to return anything
            return Ok(string.IsNullOrEmpty(id) ? "No user created" : id);
        }

        [HttpGet("{surveyId}")]
        public UserScheduleDto GetScheduleByToken(string surveyId, string token)
        {
            var response = _service.GetScheduleByToken(surveyId, token);

            return response;
        }
    }
}
