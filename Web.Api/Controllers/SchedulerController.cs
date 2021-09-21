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
        private readonly ISchedulerService _service;

        public SchedulerController(ISchedulerService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> SaveUserSchedule([FromBody] UserScheduleDto dto)
        {
            var id = await _service.SaveUserSchedule(dto);

            return Ok(id);
        }

        [HttpPost("init")]
        public async Task<IActionResult> InitUserSchedule([FromBody] UserScheduleDto dto)
        {
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] User started registration survey: {dto.FirstName} {dto.LastName} ({dto.Email})");

            var id = await _service.InitUserSchedule(dto);

            if (string.IsNullOrEmpty(id))
            {
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] User already found in scheduling system");
                return Ok("No user created");
            }

            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Initialised new user {dto.FirstName} {dto.LastName} ({dto.Email}) - Internal Id: {id}");
            return Ok(id);
        }

        [HttpGet("{surveyId}")]
        public async Task<UserScheduleDto> GetScheduleByToken(string surveyId, string token)
        {
            var response = await _service.GetScheduleByToken(surveyId, token);

            return response;
        }

        [HttpGet("mailtest/{followupDate}")]
        public async Task<IActionResult> MailTest(string followupDate, string token, string surveyId)
        {
            await _service.DebugMailTest(followupDate, token, surveyId);

            return Ok();
        }
    }
}
