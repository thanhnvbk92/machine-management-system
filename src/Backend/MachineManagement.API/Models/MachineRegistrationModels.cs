using System;

namespace MachineManagement.API.Models
{
    public class MachineRegistrationRequest
    {
        public string IP { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public string? MachineName { get; set; }
        public string? AppVersion { get; set; }
        public DateTime RequestTime { get; set; } = DateTime.Now;
    }

    public class MacUpdateRequest
    {
        public string IP { get; set; } = string.Empty;
        public string NewMacAddress { get; set; } = string.Empty;
        public string? MachineName { get; set; }
        public string? AppVersion { get; set; }
    }

    public class MacUpdateResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public MachineDetailDto? MachineInfo { get; set; }
    }

    public class MachineDetailDto
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? MachineTypeId { get; set; }
        public string IP { get; set; } = string.Empty;
        public string? GMES_Name { get; set; }
        public int? StationID { get; set; }
        public string? ProgramName { get; set; }
        public string MacAddress { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public string LineName { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string MachineTypeName { get; set; } = string.Empty;
    }

    public class MachineRegistrationResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public MachineDetailDto? MachineInfo { get; set; }
        public bool IsNewMachine { get; set; }
        public bool RequiresMacUpdate { get; set; } = false;
        public MachineDetailDto? ExistingMachine { get; set; }
    }
}