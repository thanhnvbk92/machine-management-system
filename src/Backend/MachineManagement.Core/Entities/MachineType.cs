using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("machinetypes")]
    public class MachineType
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(45)]
        public string Name { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();
    }
}