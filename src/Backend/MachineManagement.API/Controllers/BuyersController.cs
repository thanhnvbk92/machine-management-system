using Microsoft.AspNetCore.Mvc;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;
using MachineManagement.API.DTOs;

namespace MachineManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuyersController : ControllerBase
    {
        private readonly IBuyerRepository _buyerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BuyersController> _logger;

        public BuyersController(
            IBuyerRepository buyerRepository,
            IUnitOfWork unitOfWork,
            ILogger<BuyersController> logger)
        {
            _buyerRepository = buyerRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: api/buyers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuyerDto>>> GetBuyers()
        {
            try
            {
                var buyers = await _buyerRepository.GetBuyersWithModelGroupsAsync();
                var buyerDtos = buyers.Select(b => new BuyerDto
                {
                    Id = b.Id,
                    Code = b.Code,
                    Name = b.Name,
                    ModelGroupCount = b.ModelGroups.Count
                });
                
                return Ok(buyerDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving buyers");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/buyers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BuyerDto>> GetBuyer(int id)
        {
            try
            {
                var buyer = await _buyerRepository.GetBuyerWithModelGroupsAsync(id);
                if (buyer == null)
                {
                    return NotFound($"Buyer with ID {id} not found");
                }

                var buyerDto = new BuyerDto
                {
                    Id = buyer.Id,
                    Code = buyer.Code,
                    Name = buyer.Name,
                    ModelGroupCount = buyer.ModelGroups.Count
                };

                return Ok(buyerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving buyer {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/buyers
        [HttpPost]
        public async Task<ActionResult<BuyerDto>> CreateBuyer(CreateBuyerDto createBuyerDto)
        {
            try
            {
                // Check if buyer with same code exists
                if (await _buyerRepository.ExistsByCodeAsync(createBuyerDto.Code))
                {
                    return BadRequest($"Buyer with code '{createBuyerDto.Code}' already exists");
                }

                // Check if buyer with same name exists
                if (await _buyerRepository.ExistsByNameAsync(createBuyerDto.Name))
                {
                    return BadRequest($"Buyer with name '{createBuyerDto.Name}' already exists");
                }

                var buyer = new Buyer
                {
                    Code = createBuyerDto.Code,
                    Name = createBuyerDto.Name
                };

                await _buyerRepository.AddAsync(buyer);
                await _unitOfWork.SaveChangesAsync();

                var buyerDto = new BuyerDto
                {
                    Id = buyer.Id,
                    Code = buyer.Code,
                    Name = buyer.Name,
                    ModelGroupCount = 0
                };

                return CreatedAtAction(nameof(GetBuyer), new { id = buyer.Id }, buyerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating buyer");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/buyers/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBuyer(int id, UpdateBuyerDto updateBuyerDto)
        {
            try
            {
                var buyer = await _buyerRepository.GetByIdAsync(id);
                if (buyer == null)
                {
                    return NotFound($"Buyer with ID {id} not found");
                }

                // Check if another buyer with same code exists
                var existingByCode = await _buyerRepository.FirstOrDefaultAsync(b => b.Code == updateBuyerDto.Code && b.Id != id);
                if (existingByCode != null)
                {
                    return BadRequest($"Another buyer with code '{updateBuyerDto.Code}' already exists");
                }

                // Check if another buyer with same name exists
                var existingByName = await _buyerRepository.FirstOrDefaultAsync(b => b.Name == updateBuyerDto.Name && b.Id != id);
                if (existingByName != null)
                {
                    return BadRequest($"Another buyer with name '{updateBuyerDto.Name}' already exists");
                }

                buyer.Code = updateBuyerDto.Code;
                buyer.Name = updateBuyerDto.Name;

                _buyerRepository.Update(buyer);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating buyer {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/buyers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBuyer(int id)
        {
            try
            {
                var buyer = await _buyerRepository.GetBuyerWithModelGroupsAsync(id);
                if (buyer == null)
                {
                    return NotFound($"Buyer with ID {id} not found");
                }

                // Check if buyer has model groups
                if (buyer.ModelGroups.Any())
                {
                    return BadRequest($"Cannot delete buyer '{buyer.Name}' because it has {buyer.ModelGroups.Count} model groups");
                }

                _buyerRepository.Remove(buyer);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting buyer {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/buyers/search?term={searchTerm}
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BuyerDto>>> SearchBuyers([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return BadRequest("Search term is required");
                }

                var buyers = await _buyerRepository.SearchByNameOrCodeAsync(term);
                var buyerDtos = buyers.Select(b => new BuyerDto
                {
                    Id = b.Id,
                    Code = b.Code,
                    Name = b.Name,
                    ModelGroupCount = b.ModelGroups.Count
                });

                return Ok(buyerDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching buyers with term: {Term}", term);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/buyers/{id}/modelgroups
        [HttpGet("{id}/modelgroups")]
        public async Task<ActionResult<IEnumerable<object>>> GetBuyerModelGroups(int id)
        {
            try
            {
                var buyer = await _buyerRepository.GetBuyerWithModelGroupsAsync(id);
                if (buyer == null)
                {
                    return NotFound($"Buyer with ID {id} not found");
                }

                var modelGroups = buyer.ModelGroups.Select(mg => new
                {
                    mg.Id,
                    mg.Name
                });

                return Ok(modelGroups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving model groups for buyer {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}