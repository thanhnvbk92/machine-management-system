using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MachineManagement.API.DTOs;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces.Services;
using System.ComponentModel.DataAnnotations;

namespace MachineManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MachinesController : ControllerBase
    {
        private readonly IMachineService _machineService;
        private readonly IMapper _mapper;
        private readonly ILogger<MachinesController> _logger;

        public MachinesController(IMachineService machineService, IMapper mapper, ILogger<MachinesController> logger)
        {
            _machineService = machineService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Register a new machine
        /// </summary>
        /// <param name="createMachineDto">Machine registration data</param>
        /// <returns>Registered machine information</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(MachineDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<MachineDto>> RegisterMachine([FromBody] CreateMachineDto createMachineDto)
        {
            try
            {
                _logger.LogInformation("Registering machine: {MachineCode}", createMachineDto.MachineCode);

                var machine = _mapper.Map<Machine>(createMachineDto);
                var registeredMachine = await _machineService.RegisterMachineAsync(machine);
                var result = _mapper.Map<MachineDto>(registeredMachine);

                return CreatedAtAction(nameof(GetMachine), new { id = registeredMachine.MachineId }, result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid input for machine registration");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering machine");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Send heartbeat for a machine
        /// </summary>
        /// <param name="heartbeatDto">Heartbeat data</param>
        /// <returns>Updated machine information</returns>
        [HttpPost("heartbeat")]
        [ProducesResponseType(typeof(MachineDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<MachineDto>> Heartbeat([FromBody] HeartbeatDto heartbeatDto)
        {
            try
            {
                _logger.LogDebug("Heartbeat received for machine: {MachineId}", heartbeatDto.MachineId);

                var updatedMachine = await _machineService.UpdateHeartbeatAsync(heartbeatDto.MachineId);
                var result = _mapper.Map<MachineDto>(updatedMachine);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Machine not found for heartbeat");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing heartbeat");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get all machines
        /// </summary>
        /// <returns>List of all machines</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MachineDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<MachineDto>>> GetAllMachines()
        {
            try
            {
                var machines = await _machineService.GetAllMachinesAsync();
                var result = _mapper.Map<IEnumerable<MachineDto>>(machines);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all machines");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get machine by ID
        /// </summary>
        /// <param name="id">Machine ID</param>
        /// <returns>Machine details</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MachineDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<MachineDto>> GetMachine(int id)
        {
            try
            {
                var machine = await _machineService.GetMachineByIdAsync(id);
                if (machine == null)
                {
                    return NotFound($"Machine with ID {id} not found");
                }

                var result = _mapper.Map<MachineDto>(machine);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting machine by ID {MachineId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update machine information
        /// </summary>
        /// <param name="id">Machine ID</param>
        /// <param name="updateMachineDto">Updated machine data</param>
        /// <returns>Updated machine information</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(MachineDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<MachineDto>> UpdateMachine(int id, [FromBody] UpdateMachineDto updateMachineDto)
        {
            try
            {
                var existingMachine = await _machineService.GetMachineByIdAsync(id);
                if (existingMachine == null)
                {
                    return NotFound($"Machine with ID {id} not found");
                }

                _mapper.Map(updateMachineDto, existingMachine);
                var updatedMachine = await _machineService.UpdateMachineAsync(existingMachine);
                var result = _mapper.Map<MachineDto>(updatedMachine);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating machine with ID {MachineId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete machine
        /// </summary>
        /// <param name="id">Machine ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteMachine(int id)
        {
            try
            {
                var deleted = await _machineService.DeleteMachineAsync(id);
                if (!deleted)
                {
                    return NotFound($"Machine with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting machine with ID {MachineId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}