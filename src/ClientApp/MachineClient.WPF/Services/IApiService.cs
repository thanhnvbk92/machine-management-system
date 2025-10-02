using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MachineClient.WPF.Models;

namespace MachineClient.WPF.Services
{
    public interface IApiService
    {
        Task<bool> TestConnectionAsync();
        Task<MachineRegistrationResponse> RegisterMachineAsync(MachineRegistrationRequest request);
        Task<MacUpdateResponse> UpdateMacAddressAsync(MacUpdateRequest request);
        Task SendHeartbeatAsync(string machineId);
        Task SendLogsAsync(IEnumerable<LogData> logs);
        Task<IEnumerable<Command>> GetPendingCommandsAsync(string machineId);
        Task UpdateCommandStatusAsync(int commandId, string status, string? result = null);
        Task<ClientConfiguration?> GetConfigurationAsync(string machineId);
        
        // Legacy method for backward compatibility
        [Obsolete("Use RegisterMachineAsync(MachineRegistrationRequest) instead")]
        Task RegisterMachineAsync(Machine machine);
    }
}