using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("log_file")]
    public class LogFile : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;
        
        public long FileSize { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string FileType { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";
        
        public DateTime? ProcessedAt { get; set; }
        
        public string? ErrorMessage { get; set; }
        
        // Navigation properties
        public virtual ICollection<LogData> LogData { get; set; } = new List<LogData>();
    }
}