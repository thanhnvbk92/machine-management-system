namespace MachineManagerApp.Models
{
    public class Command
    {
        public string Type { get; set; } = string.Empty;  // START, STOP, RESTART, etc.
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Parameters { get; set; } = string.Empty;
    }

    public class CommandResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ErrorMessage => Success ? string.Empty : Message;  // Compatibility property
        public DateTime Timestamp { get; set; }
        public string MachineId { get; set; } = string.Empty;
    }
}