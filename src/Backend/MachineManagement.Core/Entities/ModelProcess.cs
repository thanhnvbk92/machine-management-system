using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("modelprocesses")]
    public class ModelProcess
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [ForeignKey(nameof(ModelGroup))]
        [Column("ModelGroupID")]
        public int ModelGroupId { get; set; }
        
        // Navigation properties
        public virtual ModelGroup ModelGroup { get; set; } = null!;
        public virtual ICollection<Station> Stations { get; set; } = new List<Station>();
    }
}