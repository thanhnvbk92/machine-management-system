using Microsoft.AspNetCore.Mvc;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;
using MachineManagement.API.DTOs;

namespace MachineManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StationsController : ControllerBase
    {
        private readonly IStationRepository _stationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StationsController> _logger;

        public StationsController(
            IStationRepository stationRepository,
            IUnitOfWork unitOfWork,
            ILogger<StationsController> logger)
        {
            _stationRepository = stationRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StationDto>>> GetStations()
        {
            try
            {
                var stations = await _stationRepository.GetAllAsync();
                var stationDtos = stations.Select(s => new StationDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    LineId = s.LineId,
                    ModelProcessId = s.ModelProcessId
                });

                return Ok(stationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stations");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StationDto>> GetStation(int id)
        {
            try
            {
                var station = await _stationRepository.GetByIdAsync(id);
                if (station == null)
                {
                    return NotFound();
                }

                var stationDto = new StationDto
                {
                    Id = station.Id,
                    Name = station.Name,
                    LineId = station.LineId,
                    ModelProcessId = station.ModelProcessId
                };

                return Ok(stationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving station with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<StationDto>> CreateStation(CreateStationDto createStationDto)
        {
            try
            {
                var station = new Station
                {
                    Name = createStationDto.Name,
                    LineId = createStationDto.LineId,
                    ModelProcessId = createStationDto.ModelProcessId
                };

                await _stationRepository.AddAsync(station);
                await _unitOfWork.SaveChangesAsync();

                var stationDto = new StationDto
                {
                    Id = station.Id,
                    Name = station.Name,
                    LineId = station.LineId,
                    ModelProcessId = station.ModelProcessId
                };

                return CreatedAtAction(nameof(GetStation), new { id = station.Id }, stationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating station");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStation(int id, UpdateStationDto updateStationDto)
        {
            try
            {
                var station = await _stationRepository.GetByIdAsync(id);
                if (station == null)
                {
                    return NotFound();
                }

                station.Name = updateStationDto.Name;
                station.LineId = updateStationDto.LineId;
                station.ModelProcessId = updateStationDto.ModelProcessId;

                _stationRepository.Update(station);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating station with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStation(int id)
        {
            try
            {
                var station = await _stationRepository.GetByIdAsync(id);
                if (station == null)
                {
                    return NotFound();
                }

                _stationRepository.Remove(station);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting station with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}