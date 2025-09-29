using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MachineManagement.API.DTOs;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces.Services;

namespace MachineManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly IMapper _mapper;
        private readonly ILogger<LogsController> _logger;

        public LogsController(ILogService logService, IMapper mapper, ILogger<LogsController> logger)
        {
            _logService = logService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Upload log batch from a machine
        /// </summary>
        /// <param name="logBatchDto">Batch of log entries</param>
        /// <returns>Success status</returns>
        [HttpPost("batch")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UploadLogBatch([FromBody] LogBatchDto logBatchDto)
        {
            try
            {
                if (logBatchDto?.LogEntries == null || !logBatchDto.LogEntries.Any())
                {
                    return BadRequest("Log batch cannot be empty");
                }

                _logger.LogInformation("Receiving log batch with {Count} entries", logBatchDto.LogEntries.Count);

                var logEntities = _mapper.Map<IEnumerable<LogData>>(logBatchDto.LogEntries);
                await _logService.AddLogBatchAsync(logEntities);

                return Ok(new { Message = "Log batch processed successfully", Count = logBatchDto.LogEntries.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing log batch");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Add a single log entry
        /// </summary>
        /// <param name="createLogDto">Log entry data</param>
        /// <returns>Created log entry</returns>
        [HttpPost]
        [ProducesResponseType(typeof(LogDataDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<LogDataDto>> AddLog([FromBody] CreateLogDataDto createLogDto)
        {
            try
            {
                var logEntity = _mapper.Map<LogData>(createLogDto);
                var createdLog = await _logService.AddLogAsync(logEntity);
                var result = _mapper.Map<LogDataDto>(createdLog);

                return CreatedAtAction(nameof(GetLog), new { id = createdLog.LogId }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding log entry");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Query logs with filters
        /// </summary>
        /// <param name="queryDto">Query parameters</param>
        /// <returns>Filtered log entries</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LogDataDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<LogDataDto>>> GetLogs([FromQuery] LogQueryDto queryDto)
        {
            try
            {
                if (queryDto.PageSize > 1000)
                {
                    return BadRequest("Page size cannot exceed 1000");
                }

                var logs = await _logService.GetLogsByFilterAsync(
                    queryDto.MachineCode,
                    queryDto.LogLevel,
                    queryDto.FromDate,
                    queryDto.ToDate,
                    queryDto.PageNumber,
                    queryDto.PageSize);

                var result = _mapper.Map<IEnumerable<LogDataDto>>(logs);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying logs");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get log by ID
        /// </summary>
        /// <param name="id">Log ID</param>
        /// <returns>Log details</returns>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(LogDataDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<LogDataDto>> GetLog(long id)
        {
            try
            {
                var log = await _logService.GetLogByIdAsync(id);
                if (log == null)
                {
                    return NotFound($"Log with ID {id} not found");
                }

                var result = _mapper.Map<LogDataDto>(log);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting log by ID {LogId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get logs for a specific machine
        /// </summary>
        /// <param name="machineId">Machine ID</param>
        /// <param name="fromDate">Start date filter</param>
        /// <param name="toDate">End date filter</param>
        /// <returns>Machine logs</returns>
        [HttpGet("machine/{machineId:int}")]
        [ProducesResponseType(typeof(IEnumerable<LogDataDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<LogDataDto>>> GetMachineLogs(
            int machineId, 
            [FromQuery] DateTime? fromDate = null, 
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var logs = await _logService.GetLogsByMachineIdAsync(machineId, fromDate, toDate);
                var result = _mapper.Map<IEnumerable<LogDataDto>>(logs);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting logs for machine {MachineId}", machineId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete log entry
        /// </summary>
        /// <param name="id">Log ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteLog(long id)
        {
            try
            {
                var deleted = await _logService.DeleteLogAsync(id);
                if (!deleted)
                {
                    return NotFound($"Log with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting log with ID {LogId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete old logs
        /// </summary>
        /// <param name="olderThanDays">Delete logs older than this many days</param>
        /// <returns>Success status</returns>
        [HttpDelete("cleanup/{olderThanDays:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteOldLogs(int olderThanDays)
        {
            try
            {
                if (olderThanDays <= 0)
                {
                    return BadRequest("Days must be a positive number");
                }

                var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
                await _logService.DeleteOldLogsAsync(cutoffDate);

                return Ok(new { Message = $"Old logs older than {olderThanDays} days have been deleted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting old logs");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}