using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;
    }
}