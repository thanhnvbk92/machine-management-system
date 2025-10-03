using Microsoft.AspNetCore.Mvc;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;
using MachineManagement.API.DTOs;

namespace MachineManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LinesController : ControllerBase
    {
        private readonly ILineRepository _lineRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LinesController> _logger;

        public LinesController(
            ILineRepository lineRepository,
            IUnitOfWork unitOfWork,
            ILogger<LinesController> logger)
        {
            _lineRepository = lineRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LineDto>>> GetLines()
        {
            try
            {
                var lines = await _lineRepository.GetAllAsync();
                var lineDtos = lines.Select(l => new LineDto
                {
                    Id = l.Id,
                    Name = l.Name
                });

                return Ok(lineDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lines");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LineDto>> GetLine(int id)
        {
            try
            {
                var line = await _lineRepository.GetByIdAsync(id);
                if (line == null)
                {
                    return NotFound();
                }

                var lineDto = new LineDto
                {
                    Id = line.Id,
                    Name = line.Name
                };

                return Ok(lineDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving line with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<LineDto>> CreateLine(CreateLineDto createLineDto)
        {
            try
            {
                var line = new Line
                {
                    Name = createLineDto.Name
                };

                await _lineRepository.AddAsync(line);
                await _unitOfWork.SaveChangesAsync();

                var lineDto = new LineDto
                {
                    Id = line.Id,
                    Name = line.Name
                };

                return CreatedAtAction(nameof(GetLine), new { id = line.Id }, lineDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating line");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLine(int id, UpdateLineDto updateLineDto)
        {
            try
            {
                var line = await _lineRepository.GetByIdAsync(id);
                if (line == null)
                {
                    return NotFound();
                }

                line.Name = updateLineDto.Name;

                _lineRepository.Update(line);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating line with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLine(int id)
        {
            try
            {
                var line = await _lineRepository.GetByIdAsync(id);
                if (line == null)
                {
                    return NotFound();
                }

                _lineRepository.Remove(line);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting line with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}