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
    public class TelemetryController : ControllerBase
    {
        private readonly ITelemetryService _service;

        public TelemetryController(ITelemetryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<List<string>> GetTelemetryData()
        {
            return await _service.GetData();
        }
    }
}
