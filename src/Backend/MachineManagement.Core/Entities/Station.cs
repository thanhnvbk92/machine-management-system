using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("stations")]
    public class Station : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [ForeignKey(nameof(ModelProcess))]
        public int ModelProcessId { get; set; }
        
        [Required]
        [ForeignKey(nameof(Line))]
        public int LineId { get; set; }
        
        // Navigation properties
        public virtual Line Line { get; set; } = null!;
        public virtual ModelProcess ModelProcess { get; set; } = null!;
        public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();
        public virtual ICollection<Command> Commands { get; set; } = new List<Command>();
        public virtual ICollection<LogData> LogData { get; set; } = new List<LogData>();
    }
}