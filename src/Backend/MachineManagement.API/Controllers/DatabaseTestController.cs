using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace MachineManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatabaseTestController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseTestController> _logger;

    public DatabaseTestController(IConfiguration configuration, ILogger<DatabaseTestController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet("connection")]
    public async Task<ActionResult<object>> TestConnection()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            
            _logger.LogInformation("Database connection successful");
            return Ok(new { 
                message = "Database connection successful!", 
                database = connection.Database,
                serverVersion = connection.ServerVersion
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection failed");
            return StatusCode(500, new { error = "Database connection failed", details = ex.Message });
        }
    }

    [HttpGet("tables")]
    public async Task<ActionResult<object>> ListTables()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            
            using var command = new MySqlCommand("SHOW TABLES", connection);
            var tables = new List<string>();
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tables.Add(reader.GetString(0));
            }
            
            _logger.LogInformation($"Found {tables.Count} tables");
            return Ok(new { 
                message = "Database tables retrieved successfully!", 
                tableCount = tables.Count,
                tables = tables
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list tables");
            return StatusCode(500, new { error = "Failed to list tables", details = ex.Message });
        }
    }

    [HttpGet("machines/count")]
    public async Task<ActionResult<object>> GetMachineCount()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            
            using var command = new MySqlCommand("SELECT COUNT(*) FROM machines", connection);
            var count = await command.ExecuteScalarAsync();
            
            _logger.LogInformation($"Found {count} machines");
            return Ok(new { 
                message = "Machine count retrieved successfully!", 
                count = count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get machine count");
            return StatusCode(500, new { error = "Failed to get machine count", details = ex.Message });
        }
    }
    
    
    [HttpGet("machines/columns")]
    public async Task<ActionResult<object>> GetMachinesColumns()
    {
        try
        {
            _logger.LogInformation("Getting machines table column information");
            
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            
            using var command = new MySqlCommand("SHOW COLUMNS FROM machines", connection);
            var reader = await command.ExecuteReaderAsync();
            
            var columns = new List<object>();
            while (await reader.ReadAsync())
            {
                columns.Add(new
                {
                    Field = reader["Field"]?.ToString(),
                    Type = reader["Type"]?.ToString(),
                    Null = reader["Null"]?.ToString(),
                    Key = reader["Key"]?.ToString(),
                    Default = reader["Default"]?.ToString(),
                    Extra = reader["Extra"]?.ToString()
                });
            }
            
            return Ok(new { 
                table = "machines",
                columns = columns
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get machines columns");
            return StatusCode(500, new { error = "Failed to get machines columns", details = ex.Message });
        }
    }
    
    [HttpGet("tables/{tableName}")]
    public async Task<ActionResult<object>> GetTableStructure(string tableName)
    {
        try
        {
            _logger.LogInformation($"Getting structure for table: {tableName}");
            
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            
            using var command = new MySqlCommand($"DESCRIBE {tableName}", connection);
            var reader = await command.ExecuteReaderAsync();
            
            var columns = new List<object>();
            while (await reader.ReadAsync())
            {
                columns.Add(new
                {
                    Field = reader["Field"],
                    Type = reader["Type"],
                    Null = reader["Null"],
                    Key = reader["Key"],
                    Default = reader["Default"],
                    Extra = reader["Extra"]
                });
            }
            
            return Ok(new { 
                table = tableName,
                columns = columns
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get table structure for {TableName}", tableName);
            return StatusCode(500, new { error = "Failed to get table structure", details = ex.Message });
        }
    }
}