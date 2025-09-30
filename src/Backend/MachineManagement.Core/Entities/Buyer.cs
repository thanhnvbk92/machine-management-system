using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("buyers")]
    public class Buyer : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual ICollection<ModelGroup> ModelGroups { get; set; } = new List<ModelGroup>();
    }
}