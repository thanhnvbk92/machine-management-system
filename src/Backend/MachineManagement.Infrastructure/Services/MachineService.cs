using Microsoft.Extensions.Logging;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;
using MachineManagement.Core.Interfaces.Services;

namespace MachineManagement.Infrastructure.Services
{
    public class MachineService : IMachineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MachineService> _logger;

        public MachineService(IUnitOfWork unitOfWork, ILogger<MachineService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Machine> RegisterMachineAsync(Machine machine)
        {
            try
            {
                _logger.LogInformation("Registering machine: {MachineCode}", machine.MachineCode);
                
                // Check if machine already exists
                var existingMachine = await _unitOfWork.Machines.FirstOrDefaultAsync(m => m.MachineCode == machine.MachineCode);
                if (existingMachine != null)
                {
                    _logger.LogInformation("Machine {MachineCode} already exists, updating last heartbeat", machine.MachineCode);
                    existingMachine.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Machines.Update(existingMachine);
                    await _unitOfWork.SaveChangesAsync();
                    return existingMachine;
                }

                machine.CreatedAt = DateTime.UtcNow;
                machine.IsActive = true;

                await _unitOfWork.Machines.AddAsync(machine);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Machine {MachineCode} registered successfully with ID: {MachineId}", machine.MachineCode, machine.MachineId);
                return machine;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering machine {MachineCode}", machine.MachineCode);
                throw;
            }
        }

        public async Task<Machine> UpdateHeartbeatAsync(int machineId)
        {
            try
            {
                var machine = await _unitOfWork.Machines.GetByIdAsync(machineId);
                if (machine == null)
                {
                    _logger.LogWarning("Machine with ID {MachineId} not found for heartbeat update", machineId);
                    throw new ArgumentException($"Machine with ID {machineId} not found");
                }

                machine.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Machines.Update(machine);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogDebug("Heartbeat updated for machine {MachineCode}", machine.MachineCode);
                return machine;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating heartbeat for machine ID {MachineId}", machineId);
                throw;
            }
        }

        public async Task<IEnumerable<Machine>> GetAllMachinesAsync()
        {
            try
            {
                return await _unitOfWork.Machines.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all machines");
                throw;
            }
        }

        public async Task<Machine?> GetMachineByIdAsync(int id)
        {
            try
            {
                return await _unitOfWork.Machines.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting machine by ID {MachineId}", id);
                throw;
            }
        }

        public async Task<Machine?> GetMachineByCodeAsync(string machineCode)
        {
            try
            {
                return await _unitOfWork.Machines.FirstOrDefaultAsync(m => m.MachineCode == machineCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting machine by code {MachineCode}", machineCode);
                throw;
            }
        }

        public async Task<Machine> UpdateMachineAsync(Machine machine)
        {
            try
            {
                machine.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Machines.Update(machine);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Machine {MachineCode} updated successfully", machine.MachineCode);
                return machine;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating machine {MachineCode}", machine.MachineCode);
                throw;
            }
        }

        public async Task<bool> DeleteMachineAsync(int id)
        {
            try
            {
                var machine = await _unitOfWork.Machines.GetByIdAsync(id);
                if (machine == null)
                {
                    return false;
                }

                _unitOfWork.Machines.Remove(machine);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Machine with ID {MachineId} deleted successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting machine with ID {MachineId}", id);
                throw;
            }
        }
    }
}