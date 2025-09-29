using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MachineManagement.Infrastructure.Data;

namespace MachineManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class HealthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HealthController> _logger;

        public HealthController(ApplicationDbContext context, ILogger<HealthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        /// <returns>System health status</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(503)]
        public async Task<IActionResult> GetHealth()
        {
            try
            {
                var healthStatus = new
                {
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Version = "1.0.0",
                    Checks = new
                    {
                        Database = await CheckDatabaseHealthAsync(),
                        Api = "Healthy"
                    }
                };

                return Ok(healthStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                
                var unhealthyStatus = new
                {
                    Status = "Unhealthy",
                    Timestamp = DateTime.UtcNow,
                    Version = "1.0.0",
                    Error = ex.Message,
                    Checks = new
                    {
                        Database = "Unhealthy",
                        Api = "Unhealthy"
                    }
                };

                return StatusCode(503, unhealthyStatus);
            }
        }

        /// <summary>
        /// Detailed health check endpoint
        /// </summary>
        /// <returns>Detailed system health status</returns>
        [HttpGet("detailed")]
        [ProducesResponseType(200)]
        [ProducesResponseType(503)]
        public async Task<IActionResult> GetDetailedHealth()
        {
            try
            {
                var dbHealth = await CheckDatabaseHealthAsync();
                var memoryUsage = GC.GetTotalMemory(false) / 1024 / 1024; // MB

                var detailedHealth = new
                {
                    Status = dbHealth == "Healthy" ? "Healthy" : "Unhealthy",
                    Timestamp = DateTime.UtcNow,
                    Version = "1.0.0",
                    Uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime,
                    MemoryUsage = $"{memoryUsage} MB",
                    Checks = new
                    {
                        Database = new
                        {
                            Status = dbHealth,
                            ResponseTime = await MeasureDatabaseResponseTimeAsync()
                        },
                        Api = new
                        {
                            Status = "Healthy",
                            ResponseTime = "< 1ms"
                        }
                    }
                };

                return dbHealth == "Healthy" ? Ok(detailedHealth) : StatusCode(503, detailedHealth);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Detailed health check failed");
                return StatusCode(503, new { Status = "Unhealthy", Error = ex.Message });
            }
        }

        private async Task<string> CheckDatabaseHealthAsync()
        {
            try
            {
                await _context.Database.CanConnectAsync();
                return "Healthy";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database health check failed");
                return "Unhealthy";
            }
        }

        private async Task<string> MeasureDatabaseResponseTimeAsync()
        {
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                await _context.Database.CanConnectAsync();
                stopwatch.Stop();
                return $"{stopwatch.ElapsedMilliseconds} ms";
            }
            catch
            {
                return "N/A";
            }
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VersionController : ControllerBase
    {
        /// <summary>
        /// Get API version information
        /// </summary>
        /// <returns>API version details</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult GetVersion()
        {
            var version = new
            {
                ApiVersion = "1.0.0",
                BuildDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                Framework = ".NET 8.0",
                Database = "MySQL with Entity Framework Core",
                Features = new[]
                {
                    "Machine Management",
                    "Log Collection",
                    "Command Processing",
                    "Health Monitoring",
                    "RESTful API",
                    "Swagger Documentation"
                }
            };

            return Ok(version);
        }
    }
}