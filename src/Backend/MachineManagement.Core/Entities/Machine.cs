using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineManagement.Core.Entities
{
    [Table("machines")]
    public class Machine
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? Status { get; set; }
        
        public int? MachineTypeId { get; set; }
        
        [MaxLength(45)]
        [Column("IP")]
        public string? Ip { get; set; }
        
        [MaxLength(255)]
        [Column("GMES_Name")]
        public string? GmesName { get; set; }
        
        [Column("StationID")]
        public int? StationId { get; set; }
        
        [MaxLength(255)]
        public string? ProgramName { get; set; }
        
        /// <summary>
        /// MAC address of the machine
        /// </summary>
        [MaxLength(17)]
        [Column("mac_address")]
        public string? MacAddress { get; set; }
        
        [Column("last_log_time")]
        public DateTime? LastLogTime { get; set; }
        
        [MaxLength(50)]
        [Column("app_version")]
        public string? AppVersion { get; set; }
        
        [MaxLength(50)]
        [Column("client_status")]
        public string? ClientStatus { get; set; }
        
        [Column("last_seen")]
        public DateTime? LastSeen { get; set; }
        
        // Navigation properties
        public virtual Station? Station { get; set; }
        public virtual MachineType? MachineType { get; set; }
        public virtual ICollection<ClientConfig> ClientConfigs { get; set; } = new List<ClientConfig>();
        public virtual ICollection<Command> Commands { get; set; } = new List<Command>();
        public virtual ICollection<LogData> LogData { get; set; } = new List<LogData>();
    }
}