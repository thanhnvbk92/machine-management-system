using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("COMMANDS")]
    public class Command : BaseEntity
    {
        [Key]
        public int CommandId { get; set; }
        
        [Required]
        public int MachineId { get; set; }
        
        [Required, StringLength(100)]
        public string CommandType { get; set; } = string.Empty;
        
        [Required]
        public string CommandData { get; set; } = string.Empty;
        
        [Required, StringLength(20)]
        public string Status { get; set; } = "Pending";
        
        public DateTime? ExecutedAt { get; set; }
        
        public string? Response { get; set; }
        
        [StringLength(500)]
        public string? ErrorMessage { get; set; }
        
        [Required]
        public int Priority { get; set; } = 1;
        
        public DateTime? ScheduledAt { get; set; }
        
        // Navigation property
        public virtual Machine Machine { get; set; } = null!;
    }
}