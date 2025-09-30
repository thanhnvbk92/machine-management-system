using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("commands")]
    public class Command : BaseEntity
    {
        public int CommandId { get; set; }
        
        [ForeignKey(nameof(Machine))]
        public int? MachineId { get; set; }
        
        [ForeignKey(nameof(Station))]
        public int? StationId { get; set; }
        
        [Required, StringLength(255)]
        public string CommandType { get; set; } = string.Empty;
        
        [StringLength(255)]
        public string? ProgramName { get; set; }
        
        public string? Parameters { get; set; }
        
        [StringLength(50)]
        public string? Status { get; set; }
        
        public int? Priority { get; set; }
        
        public DateTime? CreatedTime { get; set; }
        
        public DateTime? SentTime { get; set; }
        
        public DateTime? ExecutedTime { get; set; }
        
        public string? ResultMessage { get; set; }
        
        // Navigation properties
        public virtual Machine? Machine { get; set; }
        public virtual Station? Station { get; set; }
    }
}