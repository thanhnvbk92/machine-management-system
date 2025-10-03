using Microsoft.AspNetCore.Mvc;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;
using MachineManagement.API.DTOs;

namespace MachineManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModelsController : ControllerBase
    {
        private readonly IModelRepository _modelRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ModelsController> _logger;

        public ModelsController(
            IModelRepository modelRepository,
            IUnitOfWork unitOfWork,
            ILogger<ModelsController> logger)
        {
            _modelRepository = modelRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModelDto>>> GetModels()
        {
            try
            {
                var models = await _modelRepository.GetAllAsync();
                var modelDtos = models.Select(m => new ModelDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    ModelGroupId = m.ModelGroupId
                });

                return Ok(modelDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving models");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ModelDto>> GetModel(int id)
        {
            try
            {
                var model = await _modelRepository.GetByIdAsync(id);
                if (model == null)
                {
                    return NotFound();
                }

                var modelDto = new ModelDto
                {
                    Id = model.Id,
                    Name = model.Name,
                    ModelGroupId = model.ModelGroupId
                };

                return Ok(modelDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving model with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ModelDto>> CreateModel(CreateModelDto createModelDto)
        {
            try
            {
                var model = new Model
                {
                    Name = createModelDto.Name,
                    ModelGroupId = createModelDto.ModelGroupId
                };

                await _modelRepository.AddAsync(model);
                await _unitOfWork.SaveChangesAsync();

                var modelDto = new ModelDto
                {
                    Id = model.Id,
                    Name = model.Name,
                    ModelGroupId = model.ModelGroupId
                };

                return CreatedAtAction(nameof(GetModel), new { id = model.Id }, modelDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating model");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateModel(int id, UpdateModelDto updateModelDto)
        {
            try
            {
                var model = await _modelRepository.GetByIdAsync(id);
                if (model == null)
                {
                    return NotFound();
                }

                model.Name = updateModelDto.Name;
                model.ModelGroupId = updateModelDto.ModelGroupId;

                _modelRepository.Update(model);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating model with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModel(int id)
        {
            try
            {
                var model = await _modelRepository.GetByIdAsync(id);
                if (model == null)
                {
                    return NotFound();
                }

                _modelRepository.Remove(model);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting model with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}