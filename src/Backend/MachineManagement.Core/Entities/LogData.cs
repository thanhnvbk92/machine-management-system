using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("LOGDATA")]
    public class LogData : BaseEntity
    {
        [Key]
        public long LogId { get; set; }
        
        [Required]
        public int MachineId { get; set; }
        
        [Required, StringLength(50)]
        public string LogType { get; set; } = string.Empty;
        
        [Required, StringLength(100)]
        public string LogLevel { get; set; } = string.Empty;
        
        [Required]
        public string Message { get; set; } = string.Empty;
        
        public string? Details { get; set; }
        
        [Required]
        public DateTime LogTimestamp { get; set; }
        
        [StringLength(100)]
        public string? Source { get; set; }
        
        [StringLength(100)]
        public string? Category { get; set; }
        
        // Navigation property
        public virtual Machine Machine { get; set; } = null!;
    }
}