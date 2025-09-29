using MachineManagement.Core.Entities;

namespace MachineManagement.Core.Interfaces.Services
{
    public interface IMachineService
    {
        Task<Machine> RegisterMachineAsync(Machine machine);
        Task<Machine> UpdateHeartbeatAsync(int machineId);
        Task<IEnumerable<Machine>> GetAllMachinesAsync();
        Task<Machine?> GetMachineByIdAsync(int id);
        Task<Machine?> GetMachineByCodeAsync(string machineCode);
        Task<Machine> UpdateMachineAsync(Machine machine);
        Task<bool> DeleteMachineAsync(int id);
    }
}