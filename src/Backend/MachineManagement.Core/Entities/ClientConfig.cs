using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("CLIENT_CONFIGS")]
    public class ClientConfig : BaseEntity
    {
        [Key]
        public int ConfigId { get; set; }
        
        [Required]
        public int MachineId { get; set; }
        
        [Required, StringLength(100)]
        public string ConfigKey { get; set; } = string.Empty;
        
        [Required]
        public string ConfigValue { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? DataType { get; set; } = "String";
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public bool IsEncrypted { get; set; } = false;
        
        // Navigation property
        public virtual Machine Machine { get; set; } = null!;
    }
}