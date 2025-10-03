using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MachineManagement.Infrastructure.Data;
using MachineManagement.Core.Entities;
using MachineManagement.API.DTOs;

namespace MachineManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModelProcessesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ModelProcessesController> _logger;

    public ModelProcessesController(ApplicationDbContext context, ILogger<ModelProcessesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/ModelProcesses
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModelProcessDto>>> GetModelProcesses()
    {
        try
        {
            _logger.LogInformation("Getting all model processes");
            
            var modelProcesses = await _context.ModelProcesses
                .Include(mp => mp.ModelGroup)
                .ThenInclude(mg => mg.Buyer)
                .ToListAsync();

            var modelProcessDtos = modelProcesses.Select(mp => new ModelProcessDto
            {
                Id = mp.Id,
                Name = mp.Name,
                ModelGroupId = mp.ModelGroupId,
                ModelGroupName = mp.ModelGroup?.Name,
                BuyerName = mp.ModelGroup?.Buyer?.Name,
                StationCount = mp.Stations?.Count ?? 0
            }).ToList();

            return Ok(modelProcessDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving model processes");
            return StatusCode(500, "Internal server error occurred while retrieving model processes");
        }
    }

    // GET: api/ModelProcesses/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ModelProcessDto>> GetModelProcess(int id)
    {
        try
        {
            _logger.LogInformation("Getting model process with ID: {Id}", id);
            
            var modelProcess = await _context.ModelProcesses
                .Include(mp => mp.ModelGroup)
                .ThenInclude(mg => mg.Buyer)
                .Include(mp => mp.Stations)
                .FirstOrDefaultAsync(mp => mp.Id == id);

            if (modelProcess == null)
            {
                _logger.LogWarning("Model process with ID {Id} not found", id);
                return NotFound($"Model process with ID {id} not found");
            }

            var modelProcessDto = new ModelProcessDto
            {
                Id = modelProcess.Id,
                Name = modelProcess.Name,
                ModelGroupId = modelProcess.ModelGroupId,
                ModelGroupName = modelProcess.ModelGroup?.Name,
                BuyerName = modelProcess.ModelGroup?.Buyer?.Name,
                StationCount = modelProcess.Stations?.Count ?? 0
            };

            return Ok(modelProcessDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving model process with ID: {Id}", id);
            return StatusCode(500, "Internal server error occurred while retrieving model process");
        }
    }

    // POST: api/ModelProcesses
    [HttpPost]
    public async Task<ActionResult<ModelProcessDto>> CreateModelProcess(CreateModelProcessDto createModelProcessDto)
    {
        try
        {
            _logger.LogInformation("Creating new model process: {Name}", createModelProcessDto.Name);

            // Validate ModelGroup exists
            var modelGroupExists = await _context.ModelGroups.AnyAsync(mg => mg.Id == createModelProcessDto.ModelGroupId);
            if (!modelGroupExists)
            {
                return BadRequest($"ModelGroup with ID {createModelProcessDto.ModelGroupId} does not exist");
            }

            var modelProcess = new ModelProcess
            {
                Name = createModelProcessDto.Name,
                ModelGroupId = createModelProcessDto.ModelGroupId
            };

            _context.ModelProcesses.Add(modelProcess);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully created model process with ID: {Id}", modelProcess.Id);

            // Reload with includes for response
            var createdModelProcess = await _context.ModelProcesses
                .Include(mp => mp.ModelGroup)
                .ThenInclude(mg => mg.Buyer)
                .FirstAsync(mp => mp.Id == modelProcess.Id);

            var modelProcessDto = new ModelProcessDto
            {
                Id = createdModelProcess.Id,
                Name = createdModelProcess.Name,
                ModelGroupId = createdModelProcess.ModelGroupId,
                ModelGroupName = createdModelProcess.ModelGroup?.Name,
                BuyerName = createdModelProcess.ModelGroup?.Buyer?.Name,
                StationCount = 0
            };

            return CreatedAtAction(nameof(GetModelProcess), new { id = modelProcess.Id }, modelProcessDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating model process: {Name}", createModelProcessDto.Name);
            return StatusCode(500, "Internal server error occurred while creating model process");
        }
    }

    // PUT: api/ModelProcesses/5
    [HttpPut("{id}")]
    public async Task<ActionResult<ModelProcessDto>> UpdateModelProcess(int id, UpdateModelProcessDto updateModelProcessDto)
    {
        try
        {
            _logger.LogInformation("Updating model process with ID: {Id}", id);

            var modelProcess = await _context.ModelProcesses.FindAsync(id);
            if (modelProcess == null)
            {
                _logger.LogWarning("Model process with ID {Id} not found for update", id);
                return NotFound($"Model process with ID {id} not found");
            }

            // Validate ModelGroup exists if provided
            if (updateModelProcessDto.ModelGroupId.HasValue)
            {
                var modelGroupExists = await _context.ModelGroups.AnyAsync(mg => mg.Id == updateModelProcessDto.ModelGroupId.Value);
                if (!modelGroupExists)
                {
                    return BadRequest($"ModelGroup with ID {updateModelProcessDto.ModelGroupId.Value} does not exist");
                }
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateModelProcessDto.Name))
                modelProcess.Name = updateModelProcessDto.Name;
            
            if (updateModelProcessDto.ModelGroupId.HasValue)
                modelProcess.ModelGroupId = updateModelProcessDto.ModelGroupId.Value;

            // Entity saved automatically with SaveChangesAsync

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully updated model process with ID: {Id}", id);

            // Reload with includes for response
            var updatedModelProcess = await _context.ModelProcesses
                .Include(mp => mp.ModelGroup)
                .ThenInclude(mg => mg.Buyer)
                .Include(mp => mp.Stations)
                .FirstAsync(mp => mp.Id == id);

            var modelProcessDto = new ModelProcessDto
            {
                Id = updatedModelProcess.Id,
                Name = updatedModelProcess.Name,
                ModelGroupId = updatedModelProcess.ModelGroupId,
                ModelGroupName = updatedModelProcess.ModelGroup?.Name,
                BuyerName = updatedModelProcess.ModelGroup?.Buyer?.Name,
                StationCount = updatedModelProcess.Stations?.Count ?? 0
            };

            return Ok(modelProcessDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating model process with ID: {Id}", id);
            return StatusCode(500, "Internal server error occurred while updating model process");
        }
    }

    // DELETE: api/ModelProcesses/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteModelProcess(int id)
    {
        try
        {
            _logger.LogInformation("Deleting model process with ID: {Id}", id);

            var modelProcess = await _context.ModelProcesses.FindAsync(id);
            if (modelProcess == null)
            {
                _logger.LogWarning("Model process with ID {Id} not found for deletion", id);
                return NotFound($"Model process with ID {id} not found");
            }

            // Check if there are dependent stations
            var hasStations = await _context.Stations.AnyAsync(s => s.ModelProcessId == id);
            if (hasStations)
            {
                return BadRequest("Cannot delete model process because it has associated stations");
            }

            _context.ModelProcesses.Remove(modelProcess);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully deleted model process with ID: {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting model process with ID: {Id}", id);
            return StatusCode(500, "Internal server error occurred while deleting model process");
        }
    }
}