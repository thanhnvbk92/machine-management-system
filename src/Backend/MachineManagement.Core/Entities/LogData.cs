using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("log_data")]
    public class LogData : BaseEntity
    {
        public int LogId { get; set; }
        
        [Required]
        [ForeignKey(nameof(LogFile))]
        public int FileId { get; set; }
        
        [Required]
        [ForeignKey(nameof(Machine))]
        public int MachineId { get; set; }
        
        [ForeignKey(nameof(Station))]
        public int? StationId { get; set; }
        
        [ForeignKey(nameof(Model))]
        public int? ModelId { get; set; }
        
        public string? Eqpinfo { get; set; }
        
        public string? Procinfo { get; set; }
        
        [Required]
        public string Pid { get; set; } = string.Empty;
        
        public string? Fid { get; set; }
        
        public string? PartNo { get; set; }
        
        public string? Variant { get; set; }
        
        [Required]
        public string Result { get; set; } = string.Empty;
        
        [Required]
        public string Jobfile { get; set; } = string.Empty;
        
        [Required]
        public string GmesStatus { get; set; } = string.Empty;
        
        [Required]
        public DateTime StartTime { get; set; }
        
        public DateTime? EndTime { get; set; }
        
        public string? StepNg { get; set; }
        
        public string? Measure { get; set; }
        
        public string? SpecMin { get; set; }
        
        public string? SpecMax { get; set; }
        
        public string? LogLevel { get; set; }
        
        public string? Source { get; set; }
        
        public string? RawData { get; set; }
        
        public DateTime? ReceivedTime { get; set; }
        
        // Navigation properties
        public virtual LogFile LogFile { get; set; } = null!;
        public virtual Machine Machine { get; set; } = null!;
        public virtual Model? Model { get; set; }
        public virtual Station? Station { get; set; }
    }
}