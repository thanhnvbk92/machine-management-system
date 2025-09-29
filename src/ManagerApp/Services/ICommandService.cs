using MachineManagement.Core.Entities;

namespace MachineManagement.ManagerApp.Services
{
    public interface ICommandService
    {
        Task<IEnumerable<Command>> GetAllCommandsAsync();
        Task<IEnumerable<Command>> GetCommandsByMachineIdAsync(int machineId);
        Task<IEnumerable<Command>> GetPendingCommandsAsync();
        Task<IEnumerable<Command>> GetRecentCommandsAsync(int count = 50);
        Task<Command> CreateCommandAsync(Command command);
        Task<Command> UpdateCommandStatusAsync(int commandId, string status, string? response = null, string? errorMessage = null);
        Task<bool> DeleteCommandAsync(int commandId);
        Task<IEnumerable<Command>> GetCommandsByStatusAsync(string status);
        Task<Dictionary<string, int>> GetCommandCountByStatusAsync();
        Task<Command?> GetCommandByIdAsync(int commandId);
        Task<int> GetPendingCommandCountByMachineAsync(int machineId);
        Task<IEnumerable<Command>> GetPagedCommandsAsync(int page, int size, string? status = null, int? machineId = null);
    }
}