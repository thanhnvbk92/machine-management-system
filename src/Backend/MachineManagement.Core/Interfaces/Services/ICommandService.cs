using MachineManagement.Core.Entities;

namespace MachineManagement.Core.Interfaces.Services
{
    public interface ICommandService
    {
        Task<Command> CreateCommandAsync(Command command);
        Task<IEnumerable<Command>> GetPendingCommandsAsync(int machineId);
        Task<Command> UpdateCommandStatusAsync(int commandId, string status, string? response = null, string? errorMessage = null);
        Task<Command?> GetCommandByIdAsync(int id);
        Task<IEnumerable<Command>> GetCommandsByMachineIdAsync(int machineId);
        Task<bool> DeleteCommandAsync(int id);
    }
}