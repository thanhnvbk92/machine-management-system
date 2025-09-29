using AutoMapper;
using MachineManagement.Core.Entities;
using MachineManagement.ManagerApp.Models;

namespace MachineManagement.ManagerApp.Models;

/// <summary>
/// AutoMapper profile for mapping between entities and DTOs
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Machine mappings
        CreateMap<Machine, MachineDto>()
            .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => 
                DateTime.UtcNow.Subtract(src.LastHeartbeat).TotalMinutes < 5))
            .ForMember(dest => dest.Uptime, opt => opt.MapFrom(src => 
                DateTime.UtcNow.Subtract(src.LastHeartbeat)));

        // Log mappings
        CreateMap<MachineLog, LogEntryDto>();

        // Command mappings  
        CreateMap<Command, CommandDto>();
        CreateMap<CreateCommandRequest, Command>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"));
    }
}