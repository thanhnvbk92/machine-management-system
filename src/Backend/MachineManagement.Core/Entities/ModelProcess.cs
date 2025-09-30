using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("modelprocesses")]
    public class ModelProcess : BaseEntity
    {
        [Required]
        [ForeignKey(nameof(Model))]
        public int ModelId { get; set; }
        
        [Required]
        [ForeignKey(nameof(Station))]
        public int StationId { get; set; }
        
        [Required]
        public int ProcessOrder { get; set; }
        
        public decimal? CycleTime { get; set; }
        
        public string? Description { get; set; }
        
        [Required]
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual Model Model { get; set; } = null!;
        public virtual Station Station { get; set; } = null!;
    }
}