using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("modelgroups")]
    public class ModelGroup : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [ForeignKey(nameof(Buyer))]
        public int BuyerId { get; set; }
        
        // Navigation properties
        public virtual Buyer Buyer { get; set; } = null!;
        public virtual ICollection<Model> Models { get; set; } = new List<Model>();
        public virtual ICollection<ModelProcess> ModelProcesses { get; set; } = new List<ModelProcess>();
    }
}