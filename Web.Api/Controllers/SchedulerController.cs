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
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] User started registration survey: {dto.FirstName} {dto.LastName} ({dto.Email})");

            var id = _service.InitUserSchedule(dto);

            if (string.IsNullOrEmpty(id))
            {
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] User already found in scheduling system");
                return Ok("No user created");
            }

            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Initialised new user {dto.FirstName} {dto.LastName} ({dto.Email}) - Internal Id: {id}");
            return Ok(id);
        }

        [HttpGet("{surveyId}")]
        public UserScheduleDto GetScheduleByToken(string surveyId, string token)
        {
            var response = _service.GetScheduleByToken(surveyId, token);

            return response;
        }
    }
}
