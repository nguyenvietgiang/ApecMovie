using LogServices.API.Models;
using LogServices.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LogServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Log>>> GetLogs(string collectionName, int pageNumber = 1, int pageSize = 10)
        {
            var logs = await _logService.GetLogsAsync(collectionName, pageNumber, pageSize);
            return Ok(logs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Log>> GetLogById(string collectionName, string id)
        {
            var log = await _logService.GetLogByIdAsync(collectionName, id);
            if (log == null)
            {
                return NotFound();
            }
            return Ok(log);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Log>>> SearchLogs(string collectionName, string keyword, DateTime? startDate, DateTime? endDate)
        {
            var logs = await _logService.SearchLogsAsync(collectionName, keyword, startDate, endDate);
            return Ok(logs);
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<LogStatistics>> GetStatistics(string collectionName)
        {
            var stats = await _logService.GetStatisticsAsync(collectionName);
            return Ok(stats);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLog(string collectionName, string id)
        {
            await _logService.DeleteLogAsync(collectionName, id);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Log>> CreateLog(string collectionName, Log log)
        {
            await _logService.CreateLogAsync(collectionName, log);
            return CreatedAtAction(nameof(GetLogById), new { collectionName, id = log.Id.ToString() }, log);
        }
    }
}
