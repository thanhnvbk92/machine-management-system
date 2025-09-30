using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("lines")]
    public class Line : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        [ForeignKey(nameof(Buyer))]
        public int BuyerId { get; set; }
        
        [Required]
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual Buyer Buyer { get; set; } = null!;
        public virtual ICollection<Station> Stations { get; set; } = new List<Station>();
    }
}