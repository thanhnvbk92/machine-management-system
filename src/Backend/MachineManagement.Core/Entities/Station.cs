using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("stations")]
    public class Station
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(255)]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;
        
        [Column("LineId")]
        public int? LineId { get; set; }
        
        [Column("ModelProcessId")]
        public int? ModelProcessId { get; set; }
        
        // Navigation properties
        public virtual Line? Line { get; set; }
        public virtual ModelProcess? ModelProcess { get; set; }
        public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();
        public virtual ICollection<Command> Commands { get; set; } = new List<Command>();
        public virtual ICollection<LogData> LogData { get; set; } = new List<LogData>();
    }
}