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
        private readonly ITelemetryService _telemetryService;

        public SchedulerController(ISchedulerService service, ITelemetryService telemetryService)
        {
            _service = service;
            _telemetryService = telemetryService;
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
            await _telemetryService.Log($"[{DateTime.Now.ToLongTimeString()}] User started registration survey: {dto.Token}");

            var id = await _service.InitUserSchedule(dto);

            if (string.IsNullOrEmpty(id))
            {
                await _telemetryService.Log($"[{DateTime.Now.ToLongTimeString()}] User already found in scheduling system ({dto.Token} / {dto.SurveyId})");
                return Ok("No user created");
            }

            await _telemetryService.Log($"[{DateTime.Now.ToLongTimeString()}] Initialised new user ({dto.Token} / {dto.SurveyId})");
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

        [HttpPost("cleanupusers")]
        public async Task<IActionResult> CleanupGhostUsers([FromBody] List<UserScheduleDto> users)
        {
            await _service.CleanupGhostUsers(users);
            return Ok();
        }
    }
}
