using Microsoft.AspNetCore.Mvc;

namespace MachineManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet]
        public ActionResult<object> GetHealth()
        {
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                Service = "Machine Management API"
            });
        }

        /// <summary>
        /// Database health check
        /// </summary>
        [HttpGet("database")]
        public async Task<ActionResult<object>> GetDatabaseHealth()
        {
            try
            {
                // Basic database connectivity test would go here
                // For now, return healthy status
                
                return Ok(new
                {
                    Status = "Healthy",
                    Database = "MySQL",
                    Timestamp = DateTime.UtcNow,
                    Message = "Database connection test not implemented yet"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                return StatusCode(503, new
                {
                    Status = "Unhealthy",
                    Database = "MySQL",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                });
            }
        }
    }
}