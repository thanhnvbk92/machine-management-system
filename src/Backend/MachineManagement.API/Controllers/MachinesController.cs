using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MachineManagement.Infrastructure.Data;
using MachineManagement.Core.Entities;

namespace MachineManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MachinesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MachinesController> _logger;

        public MachinesController(ApplicationDbContext context, ILogger<MachinesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all machines with their hierarchy information
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetMachines()
        {
            try
            {
                var machines = await _context.Machines
                    .Include(m => m.Station)
                        .ThenInclude(s => s.Line)
                            .ThenInclude(l => l.ModelProcess)
                                .ThenInclude(mp => mp.Model)
                                    .ThenInclude(mo => mo.ModelGroup)
                                        .ThenInclude(mg => mg.Buyer)
                    .Where(m => m.IsActive)
                    .Select(m => new
                    {
                        m.MachineId,
                        m.MachineName,
                        m.MachineCode,
                        m.MachineType,
                        m.Description,
                        Station = new
                        {
                            m.Station.StationId,
                            m.Station.StationName,
                            m.Station.StationCode
                        },
                        Line = new
                        {
                            m.Station.Line.LineId,
                            m.Station.Line.LineName,
                            m.Station.Line.LineCode
                        },
                        Process = new
                        {
                            m.Station.Line.ModelProcess.ModelProcessId,
                            m.Station.Line.ModelProcess.ProcessName,
                            m.Station.Line.ModelProcess.ProcessCode
                        },
                        Model = new
                        {
                            m.Station.Line.ModelProcess.Model.ModelId,
                            m.Station.Line.ModelProcess.Model.ModelName,
                            m.Station.Line.ModelProcess.Model.ModelCode
                        },
                        ModelGroup = new
                        {
                            m.Station.Line.ModelProcess.Model.ModelGroup.ModelGroupId,
                            m.Station.Line.ModelProcess.Model.ModelGroup.GroupName,
                            m.Station.Line.ModelProcess.Model.ModelGroup.GroupCode
                        },
                        Buyer = new
                        {
                            m.Station.Line.ModelProcess.Model.ModelGroup.Buyer.BuyerId,
                            m.Station.Line.ModelProcess.Model.ModelGroup.Buyer.BuyerName,
                            m.Station.Line.ModelProcess.Model.ModelGroup.Buyer.BuyerCode
                        },
                        m.CreatedAt,
                        m.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(machines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving machines");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get machine by ID with full hierarchy
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetMachine(int id)
        {
            try
            {
                var machine = await _context.Machines
                    .Include(m => m.Station)
                        .ThenInclude(s => s.Line)
                            .ThenInclude(l => l.ModelProcess)
                                .ThenInclude(mp => mp.Model)
                                    .ThenInclude(mo => mo.ModelGroup)
                                        .ThenInclude(mg => mg.Buyer)
                    .Where(m => m.MachineId == id && m.IsActive)
                    .Select(m => new
                    {
                        m.MachineId,
                        m.MachineName,
                        m.MachineCode,
                        m.MachineType,
                        m.Description,
                        Station = new
                        {
                            m.Station.StationId,
                            m.Station.StationName,
                            m.Station.StationCode,
                            m.Station.Description
                        },
                        Line = new
                        {
                            m.Station.Line.LineId,
                            m.Station.Line.LineName,
                            m.Station.Line.LineCode,
                            m.Station.Line.Description
                        },
                        Process = new
                        {
                            m.Station.Line.ModelProcess.ModelProcessId,
                            m.Station.Line.ModelProcess.ProcessName,
                            m.Station.Line.ModelProcess.ProcessCode,
                            m.Station.Line.ModelProcess.Description
                        },
                        Model = new
                        {
                            m.Station.Line.ModelProcess.Model.ModelId,
                            m.Station.Line.ModelProcess.Model.ModelName,
                            m.Station.Line.ModelProcess.Model.ModelCode,
                            m.Station.Line.ModelProcess.Model.Description
                        },
                        ModelGroup = new
                        {
                            m.Station.Line.ModelProcess.Model.ModelGroup.ModelGroupId,
                            m.Station.Line.ModelProcess.Model.ModelGroup.GroupName,
                            m.Station.Line.ModelProcess.Model.ModelGroup.GroupCode,
                            m.Station.Line.ModelProcess.Model.ModelGroup.Description
                        },
                        Buyer = new
                        {
                            m.Station.Line.ModelProcess.Model.ModelGroup.Buyer.BuyerId,
                            m.Station.Line.ModelProcess.Model.ModelGroup.Buyer.BuyerName,
                            m.Station.Line.ModelProcess.Model.ModelGroup.Buyer.BuyerCode,
                            m.Station.Line.ModelProcess.Model.ModelGroup.Buyer.Description
                        },
                        m.CreatedAt,
                        m.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (machine == null)
                {
                    return NotFound($"Machine with ID {id} not found");
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
        /// Get machines by buyer code
        /// </summary>
        [HttpGet("buyer/{buyerCode}")]
        public async Task<ActionResult<IEnumerable<object>>> GetMachinesByBuyer(string buyerCode)
        {
            try
            {
                var machines = await _context.Machines
                    .Include(m => m.Station)
                        .ThenInclude(s => s.Line)
                            .ThenInclude(l => l.ModelProcess)
                                .ThenInclude(mp => mp.Model)
                                    .ThenInclude(mo => mo.ModelGroup)
                                        .ThenInclude(mg => mg.Buyer)
                    .Where(m => m.IsActive && 
                               m.Station.Line.ModelProcess.Model.ModelGroup.Buyer.BuyerCode == buyerCode)
                    .Select(m => new
                    {
                        m.MachineId,
                        m.MachineName,
                        m.MachineCode,
                        m.MachineType,
                        Station = m.Station.StationName,
                        Line = m.Station.Line.LineName,
                        Process = m.Station.Line.ModelProcess.ProcessName,
                        Model = m.Station.Line.ModelProcess.Model.ModelName,
                        ModelGroup = m.Station.Line.ModelProcess.Model.ModelGroup.GroupName
                    })
                    .ToListAsync();

                return Ok(machines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving machines for buyer {BuyerCode}", buyerCode);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}