using System;
using MachineClient.WPF.Models;

namespace MachineClient.WPF.Services
{
    public class MachineConnectionResult
    {
        public bool IsSuccess { get; set; }
        public bool RequiresMacUpdate { get; set; }
        public bool IsNewMachine { get; set; }
        public string? Message { get; set; }
        public MachineDetailDto? ExistingMachine { get; set; }
        public MachineInfo? MachineInfo { get; set; }
    }

    public class MacUpdateResult
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public MachineInfo? MachineInfo { get; set; }
    }

    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        public bool IsConnected { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class MachineInfoUpdatedEventArgs : EventArgs
    {
        public MachineInfo MachineInfo { get; set; } = new();
    }

    public class MachineInfo
    {
        public string IpAddress { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public string AppVersion { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public string LineName { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string? ProgramName { get; set; }
    }
}