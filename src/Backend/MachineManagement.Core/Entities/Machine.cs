using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("MACHINES")]
    public class Machine : BaseEntity
    {
        [Key]
        public int MachineId { get; set; }
        
        [Required, StringLength(50)]
        public string MachineName { get; set; } = string.Empty;
        
        [Required, StringLength(100)]
        public string MachineCode { get; set; } = string.Empty;
        
        [Required, StringLength(50)]
        public string MachineType { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public int StationId { get; set; }
        
        // Navigation properties
        public virtual Station Station { get; set; } = null!;
        public virtual ICollection<LogData> LogData { get; set; } = new List<LogData>();
        public virtual ICollection<Command> Commands { get; set; } = new List<Command>();
    }
    
    [Table("STATIONS")]
    public class Station : BaseEntity
    {
        [Key]
        public int StationId { get; set; }
        
        [Required, StringLength(50)]
        public string StationName { get; set; } = string.Empty;
        
        [Required, StringLength(100)]
        public string StationCode { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public int LineId { get; set; }
        
        // Navigation properties
        public virtual Line Line { get; set; } = null!;
        public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();
    }
    
    [Table("LINES")]
    public class Line : BaseEntity
    {
        [Key]
        public int LineId { get; set; }
        
        [Required, StringLength(50)]
        public string LineName { get; set; } = string.Empty;
        
        [Required, StringLength(100)]
        public string LineCode { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public int ModelProcessId { get; set; }
        
        // Navigation properties
        public virtual ModelProcess ModelProcess { get; set; } = null!;
        public virtual ICollection<Station> Stations { get; set; } = new List<Station>();
    }
    
    [Table("MODELPROCESSES")]
    public class ModelProcess : BaseEntity
    {
        [Key]
        public int ModelProcessId { get; set; }
        
        [Required, StringLength(100)]
        public string ProcessName { get; set; } = string.Empty;
        
        [Required, StringLength(100)]
        public string ProcessCode { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public int ModelId { get; set; }
        
        // Navigation properties
        public virtual Model Model { get; set; } = null!;
        public virtual ICollection<Line> Lines { get; set; } = new List<Line>();
    }
    
    [Table("MODELS")]
    public class Model : BaseEntity
    {
        [Key]
        public int ModelId { get; set; }
        
        [Required, StringLength(100)]
        public string ModelName { get; set; } = string.Empty;
        
        [Required, StringLength(100)]
        public string ModelCode { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public int ModelGroupId { get; set; }
        
        // Navigation properties
        public virtual ModelGroup ModelGroup { get; set; } = null!;
        public virtual ICollection<ModelProcess> ModelProcesses { get; set; } = new List<ModelProcess>();
    }
    
    [Table("MODELGROUPS")]
    public class ModelGroup : BaseEntity
    {
        [Key]
        public int ModelGroupId { get; set; }
        
        [Required, StringLength(100)]
        public string GroupName { get; set; } = string.Empty;
        
        [Required, StringLength(100)]
        public string GroupCode { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public int BuyerId { get; set; }
        
        // Navigation properties
        public virtual Buyer Buyer { get; set; } = null!;
        public virtual ICollection<Model> Models { get; set; } = new List<Model>();
    }
    
    [Table("BUYERS")]
    public class Buyer : BaseEntity
    {
        [Key]
        public int BuyerId { get; set; }
        
        [Required, StringLength(100)]
        public string BuyerName { get; set; } = string.Empty;
        
        [Required, StringLength(100)]
        public string BuyerCode { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        // Navigation properties
        public virtual ICollection<ModelGroup> ModelGroups { get; set; } = new List<ModelGroup>();
    }
}