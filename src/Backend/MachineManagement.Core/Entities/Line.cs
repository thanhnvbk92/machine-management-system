using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("lines")]
    public class Line
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(255)]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual ICollection<Station> Stations { get; set; } = new List<Station>();
    }
}