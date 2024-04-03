using LogStorageService.Interfaces;
using LogStorageService.Models;
using Microsoft.AspNetCore.Mvc;

namespace LogStorageService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;
         
        public LogsController(ILogService logService )
        {
            _logService = logService;             
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
             
            var logs = await _logService.GetLogsAsync();
            if (!logs.Any())
                return NotFound();

            return Ok(logs);
        }      

        [HttpPost]
        public async Task<IActionResult> AddLog([FromBody] LogModel logModel)
        {
             
            var createdLog = await _logService.CreateLogAsync(logModel);
                return Ok(createdLog);            
        }     
    }
}
