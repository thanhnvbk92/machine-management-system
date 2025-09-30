using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MachineManagement.Infrastructure.Data;
using MachineManagement.Core.Entities;

namespace MachineManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MachinesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MachinesController> _logger;

    public MachinesController(ApplicationDbContext context, ILogger<MachinesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("test")]
    public ActionResult<object> TestMachines()
    {
        _logger.LogInformation("Test machines endpoint called");
        return Ok(new { message = "Machines controller is working!", timestamp = DateTime.Now });
    }

    [HttpGet("dbtest")]
    public async Task<ActionResult<object>> TestDatabase()
    {
        try
        {
            _logger.LogInformation("Testing database connection");
            await _context.Database.CanConnectAsync();
            return Ok(new { message = "Database connection successful!", timestamp = DateTime.Now });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection failed");
            return StatusCode(500, new { error = "Database connection failed", details = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Machine>>> GetMachines()
    {
        try
        {
            _logger.LogInformation("Starting GetMachines");
            
            // First try without includes
            var machines = await _context.Machines.ToListAsync();
            
            _logger.LogInformation("Found {Count} machines", machines.Count);
            return Ok(machines);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving machines: {Message}", ex.Message);
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Machine>> GetMachine(int id)
    {
        try
        {
            var machine = await _context.Machines
                .Include(m => m.Station)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (machine == null)
            {
                return NotFound();
            }

            return Ok(machine);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving machine {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Machine>> CreateMachine(Machine machine)
    {
        try
        {
            machine.CreatedAt = DateTime.UtcNow;
            machine.UpdatedAt = DateTime.UtcNow;

            _context.Machines.Add(machine);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMachine), new { id = machine.Id }, machine);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating machine");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMachine(int id, Machine machine)
    {
        if (id != machine.Id)
        {
            return BadRequest();
        }

        try
        {
            machine.UpdatedAt = DateTime.UtcNow;
            _context.Entry(machine).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await MachineExists(id))
            {
                return NotFound();
            }
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating machine {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMachine(int id)
    {
        try
        {
            var machine = await _context.Machines.FindAsync(id);
            if (machine == null)
            {
                return NotFound();
            }

            _context.Machines.Remove(machine);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting machine {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<object>> RegisterMachine([FromBody] object registrationRequest)
    {
        try
        {
            _logger.LogInformation("Machine registration request received");
            
            // For now, just return a success response with sample machine info
            // TODO: Implement actual machine registration logic
            
            return Ok(new 
            { 
                IsSuccess = true, 
                Message = "Machine registered successfully",
                MachineInfo = new {
                    ID = 1,
                    Name = "Machine-001",
                    Status = "Active",
                    BuyerName = "Toyota Vietnam",
                    LineName = "Assembly Line 1",
                    StationName = "Station A1",
                    ModelName = "Camry 2024",
                    ProgramName = "ProductionApp v1.0"
                },
                IsNewMachine = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering machine");
            return StatusCode(500, new { IsSuccess = false, Message = "Registration failed", Details = ex.Message });
        }
    }

    private async Task<bool> MachineExists(int id)
    {
        return await _context.Machines.AnyAsync(e => e.Id == id);
    }
}