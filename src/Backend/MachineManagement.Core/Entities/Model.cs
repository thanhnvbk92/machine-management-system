using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("models")]
    public class Model : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        [ForeignKey(nameof(ModelGroup))]
        public int ModelGroupId { get; set; }
        
        [Required]
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual ModelGroup ModelGroup { get; set; } = null!;
        public virtual ICollection<ModelProcess> ModelProcesses { get; set; } = new List<ModelProcess>();
        public virtual ICollection<LogData> LogData { get; set; } = new List<LogData>();
    }
}