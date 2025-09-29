using System.ComponentModel.DataAnnotations;

namespace MachineManagement.API.DTOs
{
    public class LogDataDto
    {
        public long LogId { get; set; }
        public int MachineId { get; set; }
        public string LogType { get; set; } = string.Empty;
        public string LogLevel { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime LogTimestamp { get; set; }
        public string? Source { get; set; }
        public string? Category { get; set; }
    }

    public class CreateLogDataDto
    {
        [Required]
        public int MachineId { get; set; }

        [Required, StringLength(50)]
        public string LogType { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string LogLevel { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public string? Details { get; set; }

        public DateTime? LogTimestamp { get; set; }

        [StringLength(100)]
        public string? Source { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }
    }

    public class LogBatchDto
    {
        [Required]
        public List<CreateLogDataDto> LogEntries { get; set; } = new();
    }

    public class LogQueryDto
    {
        public string? MachineCode { get; set; }
        public string? LogLevel { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }
}