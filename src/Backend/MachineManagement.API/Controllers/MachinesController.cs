using Microsoft.AspNetCore.Mvc;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;

namespace MachineManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MachinesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MachinesController> _logger;

        public MachinesController(IUnitOfWork unitOfWork, ILogger<MachinesController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get all machines
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Machine>>> GetMachines()
        {
            try
            {
                var machines = await _unitOfWork.Machines.GetAllAsync();
                return Ok(machines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving machines");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get machine by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Machine>> GetMachine(int id)
        {
            try
            {
                var machine = await _unitOfWork.Machines.GetByIdAsync(id);
                if (machine == null)
                {
                    return NotFound();
                }
                return Ok(machine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving machine {MachineId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create new machine
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Machine>> CreateMachine(Machine machine)
        {
            try
            {
                await _unitOfWork.Machines.AddAsync(machine);
                await _unitOfWork.SaveChangesAsync();
                
                return CreatedAtAction(nameof(GetMachine), new { id = machine.MachineId }, machine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating machine");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update machine
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMachine(int id, Machine machine)
        {
            if (id != machine.MachineId)
            {
                return BadRequest();
            }

            try
            {
                machine.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Machines.Update(machine);
                await _unitOfWork.SaveChangesAsync();
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating machine {MachineId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete machine
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachine(int id)
        {
            try
            {
                var machine = await _unitOfWork.Machines.GetByIdAsync(id);
                if (machine == null)
                {
                    return NotFound();
                }

                _unitOfWork.Machines.Remove(machine);
                await _unitOfWork.SaveChangesAsync();
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting machine {MachineId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get machine count
        /// </summary>
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetMachineCount()
        {
            try
            {
                var count = await _unitOfWork.Machines.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting machine count");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}