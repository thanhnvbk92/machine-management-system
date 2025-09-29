using System.ComponentModel.DataAnnotations;

namespace MachineManagement.API.DTOs
{
    public class MachineDto
    {
        public int MachineId { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public string MachineCode { get; set; } = string.Empty;
        public string MachineType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int StationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateMachineDto
    {
        [Required, StringLength(50)]
        public string MachineName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string MachineCode { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string MachineType { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public int StationId { get; set; }
    }

    public class UpdateMachineDto
    {
        [Required, StringLength(50)]
        public string MachineName { get; set; } = string.Empty;

        [StringLength(50)]
        public string MachineType { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public int StationId { get; set; }
    }

    public class HeartbeatDto
    {
        [Required]
        public int MachineId { get; set; }

        public string? AdditionalInfo { get; set; }
    }
}