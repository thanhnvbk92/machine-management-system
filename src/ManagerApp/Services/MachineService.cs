using Microsoft.EntityFrameworkCore;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;

namespace MachineManagement.ManagerApp.Services
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
                _logger.LogError(ex, "Error getting machine by ID: {MachineId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Machine>> GetMachinesByStatusAsync(string status)
        {
            try
            {
                // Note: Assuming we add Status property to Machine entity or use a different approach
                // For now, returning all machines - this should be enhanced when status tracking is implemented
                return await _unitOfWork.Machines.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting machines by status: {Status}", status);
                throw;
            }
        }

        public async Task<int> GetOnlineMachineCountAsync()
        {
            try
            {
                // This would need to be implemented based on last heartbeat or similar mechanism
                // For now, return total count
                return await _unitOfWork.Machines.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting online machine count");
                throw;
            }
        }

        public async Task<int> GetOfflineMachineCountAsync()
        {
            try
            {
                // Placeholder - implement based on actual status tracking
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting offline machine count");
                throw;
            }
        }

        public async Task<int> GetTotalMachineCountAsync()
        {
            try
            {
                return await _unitOfWork.Machines.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total machine count");
                throw;
            }
        }

        public async Task<Machine> UpdateMachineStatusAsync(int machineId, string status)
        {
            try
            {
                var machine = await _unitOfWork.Machines.GetByIdAsync(machineId);
                if (machine == null)
                    throw new ArgumentException($"Machine with ID {machineId} not found");

                // Update machine status - this would need additional properties
                machine.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Machines.Update(machine);
                await _unitOfWork.SaveChangesAsync();

                return machine;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating machine status: {MachineId}, {Status}", machineId, status);
                throw;
            }
        }

        public async Task<IEnumerable<Machine>> GetMachinesWithRecentHeartbeatAsync(int minutes = 5)
        {
            try
            {
                // Placeholder - implement based on heartbeat mechanism
                return await _unitOfWork.Machines.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting machines with recent heartbeat");
                throw;
            }
        }

        public async Task<IEnumerable<Machine>> SearchMachinesAsync(string searchTerm)
        {
            try
            {
                return await _unitOfWork.Machines.FindAsync(m => 
                    m.MachineName.Contains(searchTerm) || 
                    m.MachineCode.Contains(searchTerm) ||
                    (m.Description != null && m.Description.Contains(searchTerm)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching machines: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Machine>> GetPagedMachinesAsync(int page, int size)
        {
            try
            {
                return await _unitOfWork.Machines.GetPagedAsync(page, size);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged machines: Page {Page}, Size {Size}", page, size);
                throw;
            }
        }
    }
}