using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MachineManagement.Infrastructure.Data;
using MachineManagement.Core.Entities;

namespace MachineManagement.API.Controllers;

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

    [HttpGet("test")]
    public ActionResult<object> TestMachines()
    {
        _logger.LogInformation("Test machines endpoint called");
        return Ok(new { message = "Machines controller is working!", timestamp = DateTime.Now });
    }

    [HttpGet("dbtest")]
    public async Task<ActionResult<object>> TestDatabase()
    {
        try
        {
            _logger.LogInformation("Testing database connection");
            await _context.Database.CanConnectAsync();
            return Ok(new { message = "Database connection successful!", timestamp = DateTime.Now });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection failed");
            return StatusCode(500, new { error = "Database connection failed", details = ex.Message });
        }
    }

    [HttpGet("debug/{macOrIp}")]
    public async Task<ActionResult<object>> DebugMachine(string macOrIp)
    {
        try
        {
            _logger.LogInformation("Debug machine lookup for: {MacOrIp}", macOrIp);
            
            var machines = await _context.Machines
                .Where(m => m.MacAddress == macOrIp || m.Ip == macOrIp)
                .Select(m => new {
                    m.Id,
                    m.Name,
                    m.MacAddress,
                    m.Ip,
                    m.Status
                })
                .ToListAsync();

            return Ok(new { 
                query = macOrIp,
                count = machines.Count,
                machines = machines
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in debug lookup");
            return StatusCode(500, new { error = "Debug lookup failed", details = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Machine>>> GetMachines()
    {
        try
        {
            _logger.LogInformation("Starting GetMachines");
            
            // First try without includes
            var machines = await _context.Machines.ToListAsync();
            
            _logger.LogInformation("Found {Count} machines", machines.Count);
            return Ok(machines);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving machines: {Message}", ex.Message);
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Machine>> GetMachine(int id)
    {
        try
        {
            // Tạm thời disable Include để tránh lỗi Station mapping
            var machine = await _context.Machines
                //.Include(m => m.Station)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (machine == null)
            {
                return NotFound();
            }

            return Ok(machine);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving machine {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Machine>> CreateMachine(Machine machine)
    {
        try
        {
            _context.Machines.Add(machine);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMachine), new { id = machine.Id }, machine);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating machine");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMachine(int id, Machine machine)
    {
        if (id != machine.Id)
        {
            return BadRequest();
        }

        try
        {
            _context.Entry(machine).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await MachineExists(id))
            {
                return NotFound();
            }
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating machine {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMachine(int id)
    {
        try
        {
            var machine = await _context.Machines.FindAsync(id);
            if (machine == null)
            {
                return NotFound();
            }

            _context.Machines.Remove(machine);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting machine {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }


    [HttpPost("register")]
    public async Task<ActionResult> RegisterMachine([FromBody] MachineManagement.API.Models.MachineRegistrationRequest registrationRequest)
    {
        try
        {
            _logger.LogInformation("Machine registration request received: MAC={Mac}, IP={IP}", registrationRequest.MacAddress, registrationRequest.IP);

            // Tìm máy theo MAC address trước (MAC là unique cho từng máy vật lý)
            var machine = await _context.Machines
                .FirstOrDefaultAsync(m => m.MacAddress == registrationRequest.MacAddress);

            bool isNew = false;
            if (machine == null)
            {
                // Kiểm tra IP có bị trùng với máy khác không
                var existingMachineWithSameIP = await _context.Machines
                    .FirstOrDefaultAsync(m => m.Ip == registrationRequest.IP);

                if (existingMachineWithSameIP != null)
                {
                    // Không log warning nữa, chỉ trả về thông tin machine hiện tại
                    
                    // Tạm thời bỏ Station/Line lookup để tránh lỗi database mapping
                    var existingMachineDto = new MachineManagement.API.Models.MachineDetailDto
                    {
                        ID = existingMachineWithSameIP.Id,
                        Name = existingMachineWithSameIP.Name ?? string.Empty,
                        Status = existingMachineWithSameIP.Status ?? string.Empty,
                        MachineTypeId = existingMachineWithSameIP.MachineTypeId,
                        IP = existingMachineWithSameIP.Ip ?? string.Empty,
                        GMES_Name = existingMachineWithSameIP.GmesName,
                        StationID = existingMachineWithSameIP.StationId,
                        ProgramName = existingMachineWithSameIP.ProgramName,
                        MacAddress = existingMachineWithSameIP.MacAddress ?? string.Empty,
                        BuyerName = "Unknown Buyer",
                        LineName = "Unknown Line", 
                        StationName = "Unknown Station",
                        ModelName = string.Empty
                    };
                    
                    return Ok(new MachineManagement.API.Models.MachineRegistrationResponse
                    {
                        IsSuccess = true,
                        Message = "IP already exists with different MAC address",
                        RequiresMacUpdate = true,
                        ExistingMachine = existingMachineDto,
                        IsNewMachine = false
                    });
                }

                // Tạo mới máy
                machine = new MachineManagement.Core.Entities.Machine
                {
                    Name = registrationRequest.MachineName ?? $"MACHINE_{registrationRequest.MacAddress}",
                    MacAddress = registrationRequest.MacAddress,
                    Ip = registrationRequest.IP,
                    AppVersion = registrationRequest.AppVersion ?? "1.0.0",
                    Status = "Active"
                };
                _context.Machines.Add(machine);
                await _context.SaveChangesAsync();
                isNew = true;
                _logger.LogInformation("New machine created: MAC={MAC}, IP={IP}, ID={ID}", 
                    machine.MacAddress, machine.Ip, machine.Id);
            }
            else
            {
                // Cập nhật thông tin nếu IP đã thay đổi
                if (machine.Ip != registrationRequest.IP)
                {
                    // Kiểm tra IP mới có bị trùng không
                    var existingMachineWithNewIP = await _context.Machines
                        .FirstOrDefaultAsync(m => m.Ip == registrationRequest.IP && m.Id != machine.Id);

                    if (existingMachineWithNewIP != null)
                    {
                        // Không log warning nữa, chỉ trả về thông tin machine hiện tại
                        
                        // Lấy thông tin liên quan cho existing machine
                        var existingMachineWithRelations = await _context.Machines
                            .Include(m => m.Station)
                                .ThenInclude(s => s.Line)
                            .Include(m => m.Station)
                                .ThenInclude(s => s.ModelProcess)
                                    .ThenInclude(mp => mp.ModelGroup)
                                        .ThenInclude(mg => mg.Buyer)
                            .Include(m => m.MachineType)
                            .FirstOrDefaultAsync(m => m.Id == existingMachineWithNewIP.Id);

                        Station? updateStation = existingMachineWithRelations?.Station;
                        Line? updateLine = updateStation?.Line;
                        ModelProcess? updateModelProcess = updateStation?.ModelProcess;
                        ModelGroup? updateModelGroup = updateModelProcess?.ModelGroup;
                        Buyer? updateBuyer = updateModelGroup?.Buyer;
                        MachineType? updateMachineType = existingMachineWithRelations?.MachineType;

                        var existingMachineDto = new MachineManagement.API.Models.MachineDetailDto
                        {
                            ID = existingMachineWithNewIP.Id,
                            Name = existingMachineWithNewIP.Name ?? string.Empty,
                            Status = existingMachineWithNewIP.Status ?? string.Empty,
                            MachineTypeId = existingMachineWithNewIP.MachineTypeId,
                            IP = existingMachineWithNewIP.Ip ?? string.Empty,
                            GMES_Name = existingMachineWithNewIP.GmesName,
                            StationID = existingMachineWithNewIP.StationId,
                            ProgramName = existingMachineWithNewIP.ProgramName,
                            MacAddress = existingMachineWithNewIP.MacAddress ?? string.Empty,
                            BuyerName = updateBuyer?.Name ?? "Unknown Buyer",
                            LineName = updateLine?.Name ?? "Unknown Line",
                            StationName = updateStation?.Name ?? "Unknown Station",
                            ModelName = updateModelGroup?.Name ?? "Unknown Model",
                            MachineTypeName = updateMachineType?.Name ?? "Unknown Type"
                        };
                        
                        return Ok(new MachineManagement.API.Models.MachineRegistrationResponse
                        {
                            IsSuccess = true,
                            Message = "IP already exists with different MAC address",
                            RequiresMacUpdate = true,
                            ExistingMachine = existingMachineDto,
                            IsNewMachine = false
                        });
                    }

                    _logger.LogInformation("Updating machine IP: MAC={MAC}, Old IP={OldIP}, New IP={NewIP}", 
                        machine.MacAddress, machine.Ip, registrationRequest.IP);
                    machine.Ip = registrationRequest.IP;
                }

                // Cập nhật các thông tin khác              
                if (!string.IsNullOrEmpty(registrationRequest.AppVersion))
                    machine.AppVersion = registrationRequest.AppVersion;

                machine.LastSeen = DateTime.Now;
                machine.Status = "Active";

                await _context.SaveChangesAsync();
                // Không log update nữa - chỉ log khi tạo machine mới
            }

            // Lấy thông tin liên quan với proper includes
            var machineWithRelations = await _context.Machines
                .Include(m => m.Station)
                    .ThenInclude(s => s.Line)
                .Include(m => m.Station)
                    .ThenInclude(s => s.ModelProcess)
                        .ThenInclude(mp => mp.ModelGroup)
                            .ThenInclude(mg => mg.Buyer)
                .Include(m => m.MachineType)
                .FirstOrDefaultAsync(m => m.Id == machine.Id);

            // Fallback nếu không tìm thấy relations
            Station? station = machineWithRelations?.Station;
            Line? line = station?.Line;
            ModelProcess? modelProcess = station?.ModelProcess;
            ModelGroup? modelGroup = modelProcess?.ModelGroup;
            Buyer? buyer = modelGroup?.Buyer;
            MachineType? machineType = machineWithRelations?.MachineType;

            // Map sang DTO trả về
            var dto = new MachineManagement.API.Models.MachineDetailDto
            {
                ID = machine.Id,
                Name = machine.Name,
                Status = machine.Status ?? string.Empty,
                MachineTypeId = machine.MachineTypeId,
                IP = machine.Ip ?? string.Empty,
                GMES_Name = machine.GmesName,
                StationID = machine.StationId,
                ProgramName = machine.ProgramName,
                MacAddress = machine.MacAddress ?? string.Empty,
                BuyerName = buyer?.Name ?? "Unknown Buyer", 
                LineName = line?.Name ?? "Unknown Line",
                StationName = station?.Name ?? "Unknown Station",
                ModelName = modelGroup?.Name ?? "Unknown Model",
                MachineTypeName = machineType?.Name ?? "Unknown Type"
            };

            return Ok(new MachineManagement.API.Models.MachineRegistrationResponse
            {
                IsSuccess = true,
                Message = isNew ? "Machine registered successfully" : "Machine found",
                MachineInfo = dto,
                IsNewMachine = isNew
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering machine");
            return StatusCode(500, new { IsSuccess = false, Message = "Registration failed", Details = ex.Message });
        }
    }

    [HttpPut("update-mac")]
    public async Task<ActionResult> UpdateMacAddress([FromBody] MachineManagement.API.Models.MacUpdateRequest updateRequest)
    {
        try
        {
            _logger.LogInformation("MAC update request received: IP={IP}, NewMAC={NewMAC}", updateRequest.IP, updateRequest.NewMacAddress);

            // Tìm machine theo IP
            var machine = await _context.Machines
                .FirstOrDefaultAsync(m => m.Ip == updateRequest.IP);

            if (machine == null)
            {
                return NotFound(new MachineManagement.API.Models.MacUpdateResponse
                {
                    IsSuccess = false,
                    Message = "Machine with specified IP not found"
                });
            }

            // Kiểm tra MAC mới có bị trùng với machine khác không
            var existingMachineWithNewMac = await _context.Machines
                .FirstOrDefaultAsync(m => m.MacAddress == updateRequest.NewMacAddress && m.Id != machine.Id);

            if (existingMachineWithNewMac != null)
            {
                return BadRequest(new MachineManagement.API.Models.MacUpdateResponse
                {
                    IsSuccess = false,
                    Message = $"MAC address {updateRequest.NewMacAddress} is already used by another machine"
                });
            }

            // Cập nhật MAC address
            var oldMac = machine.MacAddress;
            machine.MacAddress = updateRequest.NewMacAddress;
            
            // Cập nhật thông tin khác nếu có
            if (!string.IsNullOrEmpty(updateRequest.MachineName))
                machine.Name = updateRequest.MachineName;
            
            if (!string.IsNullOrEmpty(updateRequest.AppVersion))
                machine.AppVersion = updateRequest.AppVersion;

            machine.LastSeen = DateTime.Now;
            machine.Status = "Active";

            await _context.SaveChangesAsync();

            _logger.LogInformation("MAC updated successfully: IP={IP}, OldMAC={OldMAC}, NewMAC={NewMAC}, ID={ID}", 
                machine.Ip, oldMac, machine.MacAddress, machine.Id);

            // Lấy thông tin liên quan với proper includes
            var machineWithRelations = await _context.Machines
                .Include(m => m.Station)
                    .ThenInclude(s => s.Line)
                .Include(m => m.Station)
                    .ThenInclude(s => s.ModelProcess)
                        .ThenInclude(mp => mp.ModelGroup)
                            .ThenInclude(mg => mg.Buyer)
                .Include(m => m.MachineType)
                .FirstOrDefaultAsync(m => m.Id == machine.Id);

            // Fallback nếu không tìm thấy relations
            Station? station = machineWithRelations?.Station;
            Line? line = station?.Line;
            ModelProcess? modelProcess = station?.ModelProcess;
            ModelGroup? modelGroup = modelProcess?.ModelGroup;
            Buyer? buyer = modelGroup?.Buyer;
            MachineType? machineType = machineWithRelations?.MachineType;

            // Map sang DTO trả về
            var dto = new MachineManagement.API.Models.MachineDetailDto
            {
                ID = machine.Id,
                Name = machine.Name,
                Status = machine.Status ?? string.Empty,
                MachineTypeId = machine.MachineTypeId,
                IP = machine.Ip ?? string.Empty,
                GMES_Name = machine.GmesName,
                StationID = machine.StationId,
                ProgramName = machine.ProgramName,
                MacAddress = machine.MacAddress ?? string.Empty,
                BuyerName = buyer?.Name ?? "Unknown Buyer",
                LineName = line?.Name ?? "Unknown Line",
                StationName = station?.Name ?? "Unknown Station",
                ModelName = modelGroup?.Name ?? "Unknown Model",
                MachineTypeName = machineType?.Name ?? "Unknown Type"
            };

            return Ok(new MachineManagement.API.Models.MacUpdateResponse
            {
                IsSuccess = true,
                Message = "MAC address updated successfully",
                MachineInfo = dto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating MAC address");
            return StatusCode(500, new MachineManagement.API.Models.MacUpdateResponse
            {
                IsSuccess = false,
                Message = "MAC update failed"
            });
        }
    }

    private async Task<bool> MachineExists(int id)
    {
        return await _context.Machines.AnyAsync(e => e.Id == id);
    }
}