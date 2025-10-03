using System.ComponentModel.DataAnnotations;

namespace MachineManagement.API.DTOs
{
    // Buyer DTOs
    public class BuyerDto
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(10)]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        public int ModelGroupCount { get; set; }
    }

    public class CreateBuyerDto
    {
        [Required]
        [MaxLength(10)]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateBuyerDto
    {
        [Required]
        [MaxLength(10)]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }

    // Line DTOs
    public class LineDto
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
    }

    public class CreateLineDto
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateLineDto
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
    }

    // ModelGroup DTOs
    public class ModelGroupDto
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        public int BuyerId { get; set; }
    }

    public class CreateModelGroupDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public int BuyerId { get; set; }
    }

    public class UpdateModelGroupDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public int BuyerId { get; set; }
    }

    // Model DTOs
    public class ModelDto
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        public int ModelGroupId { get; set; }
    }

    public class CreateModelDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public int ModelGroupId { get; set; }
    }

    public class UpdateModelDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public int ModelGroupId { get; set; }
    }

    // Station DTOs
    public class StationDto
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        public int? LineId { get; set; }
        public int? ModelProcessId { get; set; }
    }

    public class CreateStationDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        public int? LineId { get; set; }
        public int? ModelProcessId { get; set; }
    }

    public class UpdateStationDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        public int? LineId { get; set; }
        public int? ModelProcessId { get; set; }
    }

    // MachineType DTOs
    public class MachineTypeDto
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(45)]
        public string Name { get; set; } = string.Empty;
    }

    public class CreateMachineTypeDto
    {
        [Required]
        [MaxLength(45)]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateMachineTypeDto
    {
        [Required]
        [MaxLength(45)]
        public string Name { get; set; } = string.Empty;
    }

    // ModelProcess DTOs
    public class ModelProcessDto
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        
        public int ModelGroupId { get; set; }
        public string? ModelGroupName { get; set; }
        public string? BuyerName { get; set; }
        
        public int StationCount { get; set; }
    }

    public class CreateModelProcessDto
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public int ModelGroupId { get; set; }
    }

    public class UpdateModelProcessDto
    {
        [MaxLength(255)]
        public string? Name { get; set; }
        
        public int? ModelGroupId { get; set; }
    }
}