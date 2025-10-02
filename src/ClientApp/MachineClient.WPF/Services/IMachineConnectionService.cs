using System;
using System.Threading.Tasks;
using MachineClient.WPF.Models;

namespace MachineClient.WPF.Services
{
    public interface IMachineConnectionService
    {
        Task<bool> TestConnectionAsync();
        Task<MachineConnectionResult> ConnectAndRegisterAsync(MachineInfo machineInfo);
        Task<MacUpdateResult> UpdateMacAddressAsync(MacUpdateRequest request);
        
        event EventHandler<ConnectionStatusChangedEventArgs>? ConnectionStatusChanged;
        event EventHandler<MachineInfoUpdatedEventArgs>? MachineInfoUpdated;
        
        bool IsConnected { get; }
        string ConnectionStatus { get; }
    }
}