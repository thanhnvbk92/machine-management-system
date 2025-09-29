using MachineClient.WPF.Models;

namespace MachineClient.WPF.Services
{
    public interface IApiService
    {
        Task<bool> TestConnectionAsync();
        Task RegisterMachineAsync(Machine machine);
        Task SendHeartbeatAsync(string machineId);
        Task SendLogsAsync(IEnumerable<LogData> logs);
        Task<IEnumerable<Command>> GetPendingCommandsAsync(string machineId);
        Task UpdateCommandStatusAsync(int commandId, string status, string? result = null);
        Task<ClientConfiguration?> GetConfigurationAsync(string machineId);
    }
}