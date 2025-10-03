using Microsoft.AspNetCore.Mvc;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;
using MachineManagement.API.DTOs;

namespace MachineManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MachineTypesController : ControllerBase
    {
        private readonly IMachineTypeRepository _machineTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MachineTypesController> _logger;

        public MachineTypesController(
            IMachineTypeRepository machineTypeRepository,
            IUnitOfWork unitOfWork,
            ILogger<MachineTypesController> logger)
        {
            _machineTypeRepository = machineTypeRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MachineTypeDto>>> GetMachineTypes()
        {
            try
            {
                var machineTypes = await _machineTypeRepository.GetAllAsync();
                var machineTypeDtos = machineTypes.Select(mt => new MachineTypeDto
                {
                    Id = mt.Id,
                    Name = mt.Name
                });

                return Ok(machineTypeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving machine types");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MachineTypeDto>> GetMachineType(int id)
        {
            try
            {
                var machineType = await _machineTypeRepository.GetByIdAsync(id);
                if (machineType == null)
                {
                    return NotFound();
                }

                var machineTypeDto = new MachineTypeDto
                {
                    Id = machineType.Id,
                    Name = machineType.Name
                };

                return Ok(machineTypeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving machine type with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<MachineTypeDto>> CreateMachineType(CreateMachineTypeDto createMachineTypeDto)
        {
            try
            {
                var machineType = new MachineType
                {
                    Name = createMachineTypeDto.Name
                };

                await _machineTypeRepository.AddAsync(machineType);
                await _unitOfWork.SaveChangesAsync();

                var machineTypeDto = new MachineTypeDto
                {
                    Id = machineType.Id,
                    Name = machineType.Name
                };

                return CreatedAtAction(nameof(GetMachineType), new { id = machineType.Id }, machineTypeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating machine type");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMachineType(int id, UpdateMachineTypeDto updateMachineTypeDto)
        {
            try
            {
                var machineType = await _machineTypeRepository.GetByIdAsync(id);
                if (machineType == null)
                {
                    return NotFound();
                }

                machineType.Name = updateMachineTypeDto.Name;

                _machineTypeRepository.Update(machineType);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating machine type with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachineType(int id)
        {
            try
            {
                var machineType = await _machineTypeRepository.GetByIdAsync(id);
                if (machineType == null)
                {
                    return NotFound();
                }

                _machineTypeRepository.Remove(machineType);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting machine type with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}