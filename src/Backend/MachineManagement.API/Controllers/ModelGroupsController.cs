using Microsoft.AspNetCore.Mvc;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;
using MachineManagement.API.DTOs;

namespace MachineManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModelGroupsController : ControllerBase
    {
        private readonly IModelGroupRepository _modelGroupRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ModelGroupsController> _logger;

        public ModelGroupsController(
            IModelGroupRepository modelGroupRepository,
            IUnitOfWork unitOfWork,
            ILogger<ModelGroupsController> logger)
        {
            _modelGroupRepository = modelGroupRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModelGroupDto>>> GetModelGroups()
        {
            try
            {
                var modelGroups = await _modelGroupRepository.GetAllAsync();
                var modelGroupDtos = modelGroups.Select(mg => new ModelGroupDto
                {
                    Id = mg.Id,
                    Name = mg.Name,
                    BuyerId = mg.BuyerId
                });

                return Ok(modelGroupDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving model groups");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ModelGroupDto>> GetModelGroup(int id)
        {
            try
            {
                var modelGroup = await _modelGroupRepository.GetByIdAsync(id);
                if (modelGroup == null)
                {
                    return NotFound();
                }

                var modelGroupDto = new ModelGroupDto
                {
                    Id = modelGroup.Id,
                    Name = modelGroup.Name,
                    BuyerId = modelGroup.BuyerId
                };

                return Ok(modelGroupDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving model group with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ModelGroupDto>> CreateModelGroup(CreateModelGroupDto createModelGroupDto)
        {
            try
            {
                var modelGroup = new ModelGroup
                {
                    Name = createModelGroupDto.Name,
                    BuyerId = createModelGroupDto.BuyerId
                };

                await _modelGroupRepository.AddAsync(modelGroup);
                await _unitOfWork.SaveChangesAsync();

                var modelGroupDto = new ModelGroupDto
                {
                    Id = modelGroup.Id,
                    Name = modelGroup.Name,
                    BuyerId = modelGroup.BuyerId
                };

                return CreatedAtAction(nameof(GetModelGroup), new { id = modelGroup.Id }, modelGroupDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating model group");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateModelGroup(int id, UpdateModelGroupDto updateModelGroupDto)
        {
            try
            {
                var modelGroup = await _modelGroupRepository.GetByIdAsync(id);
                if (modelGroup == null)
                {
                    return NotFound();
                }

                modelGroup.Name = updateModelGroupDto.Name;
                modelGroup.BuyerId = updateModelGroupDto.BuyerId;

                _modelGroupRepository.Update(modelGroup);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating model group with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModelGroup(int id)
        {
            try
            {
                var modelGroup = await _modelGroupRepository.GetByIdAsync(id);
                if (modelGroup == null)
                {
                    return NotFound();
                }

                _modelGroupRepository.Remove(modelGroup);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting model group with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}