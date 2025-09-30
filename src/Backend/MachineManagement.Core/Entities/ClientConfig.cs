using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("client_config")]
    public class ClientConfig : BaseEntity
    {
        public int ConfigId { get; set; }
        
        [ForeignKey(nameof(Machine))]
        public int? MachineId { get; set; }
        
        [Required, StringLength(255)]
        public string ConfigKey { get; set; } = string.Empty;
        
        public string? ConfigValue { get; set; }
        
        public string? Description { get; set; }
        
        public DateTime? UpdatedTime { get; set; }
        
        // Navigation property
        public virtual Machine? Machine { get; set; }
    }
}