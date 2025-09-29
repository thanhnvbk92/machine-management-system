using MachineManagement.Core.Entities;

namespace MachineManagement.ManagerApp.Services
{
    public interface IMachineService
    {
        Task<IEnumerable<Machine>> GetAllMachinesAsync();
        Task<Machine?> GetMachineByIdAsync(int id);
        Task<IEnumerable<Machine>> GetMachinesByStatusAsync(string status);
        Task<int> GetOnlineMachineCountAsync();
        Task<int> GetOfflineMachineCountAsync();
        Task<int> GetTotalMachineCountAsync();
        Task<Machine> UpdateMachineStatusAsync(int machineId, string status);
        Task<IEnumerable<Machine>> GetMachinesWithRecentHeartbeatAsync(int minutes = 5);
        Task<IEnumerable<Machine>> SearchMachinesAsync(string searchTerm);
        Task<IEnumerable<Machine>> GetPagedMachinesAsync(int page, int size);
    }
}