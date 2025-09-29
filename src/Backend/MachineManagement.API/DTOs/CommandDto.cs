using System.ComponentModel.DataAnnotations;

namespace MachineManagement.API.DTOs
{
    public class CommandDto
    {
        public int CommandId { get; set; }
        public int MachineId { get; set; }
        public string CommandType { get; set; } = string.Empty;
        public string CommandData { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? ExecutedAt { get; set; }
        public string? Response { get; set; }
        public string? ErrorMessage { get; set; }
        public int Priority { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateCommandDto
    {
        [Required]
        public int MachineId { get; set; }

        [Required, StringLength(100)]
        public string CommandType { get; set; } = string.Empty;

        [Required]
        public string CommandData { get; set; } = string.Empty;

        [Range(1, 10)]
        public int Priority { get; set; } = 1;

        public DateTime? ScheduledAt { get; set; }
    }

    public class UpdateCommandStatusDto
    {
        [Required, StringLength(20)]
        public string Status { get; set; } = string.Empty;

        public string? Response { get; set; }

        public string? ErrorMessage { get; set; }
    }
}