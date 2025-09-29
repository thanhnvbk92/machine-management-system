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
    public class CommandsController : ControllerBase
    {
        private readonly ICommandService _commandService;
        private readonly IMapper _mapper;
        private readonly ILogger<CommandsController> _logger;

        public CommandsController(ICommandService commandService, IMapper mapper, ILogger<CommandsController> logger)
        {
            _commandService = commandService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Create a new command
        /// </summary>
        /// <param name="createCommandDto">Command data</param>
        /// <returns>Created command</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CommandDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CommandDto>> CreateCommand([FromBody] CreateCommandDto createCommandDto)
        {
            try
            {
                _logger.LogInformation("Creating command for machine {MachineId}: {CommandType}", 
                    createCommandDto.MachineId, createCommandDto.CommandType);

                var command = _mapper.Map<Command>(createCommandDto);
                var createdCommand = await _commandService.CreateCommandAsync(command);
                var result = _mapper.Map<CommandDto>(createdCommand);

                return CreatedAtAction(nameof(GetCommand), new { id = createdCommand.CommandId }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating command");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get pending commands for a machine
        /// </summary>
        /// <param name="machineId">Machine ID</param>
        /// <returns>List of pending commands</returns>
        [HttpGet("pending/{machineId:int}")]
        [ProducesResponseType(typeof(IEnumerable<CommandDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CommandDto>>> GetPendingCommands(int machineId)
        {
            try
            {
                _logger.LogDebug("Getting pending commands for machine {MachineId}", machineId);

                var commands = await _commandService.GetPendingCommandsAsync(machineId);
                var result = _mapper.Map<IEnumerable<CommandDto>>(commands);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending commands for machine {MachineId}", machineId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update command status
        /// </summary>
        /// <param name="id">Command ID</param>
        /// <param name="updateStatusDto">Status update data</param>
        /// <returns>Updated command</returns>
        [HttpPut("{id:int}/status")]
        [ProducesResponseType(typeof(CommandDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CommandDto>> UpdateCommandStatus(int id, [FromBody] UpdateCommandStatusDto updateStatusDto)
        {
            try
            {
                _logger.LogInformation("Updating command {CommandId} status to {Status}", id, updateStatusDto.Status);

                var updatedCommand = await _commandService.UpdateCommandStatusAsync(
                    id, 
                    updateStatusDto.Status, 
                    updateStatusDto.Response, 
                    updateStatusDto.ErrorMessage);

                var result = _mapper.Map<CommandDto>(updatedCommand);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Command not found for status update");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating command status");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get command by ID
        /// </summary>
        /// <param name="id">Command ID</param>
        /// <returns>Command details</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CommandDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CommandDto>> GetCommand(int id)
        {
            try
            {
                var command = await _commandService.GetCommandByIdAsync(id);
                if (command == null)
                {
                    return NotFound($"Command with ID {id} not found");
                }

                var result = _mapper.Map<CommandDto>(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting command by ID {CommandId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get all commands for a machine
        /// </summary>
        /// <param name="machineId">Machine ID</param>
        /// <returns>List of commands for the machine</returns>
        [HttpGet("machine/{machineId:int}")]
        [ProducesResponseType(typeof(IEnumerable<CommandDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CommandDto>>> GetMachineCommands(int machineId)
        {
            try
            {
                var commands = await _commandService.GetCommandsByMachineIdAsync(machineId);
                var result = _mapper.Map<IEnumerable<CommandDto>>(commands);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting commands for machine {MachineId}", machineId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete command
        /// </summary>
        /// <param name="id">Command ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCommand(int id)
        {
            try
            {
                var deleted = await _commandService.DeleteCommandAsync(id);
                if (!deleted)
                {
                    return NotFound($"Command with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting command with ID {CommandId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}