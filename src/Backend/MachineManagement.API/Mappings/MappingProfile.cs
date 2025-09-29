using AutoMapper;
using MachineManagement.API.DTOs;
using MachineManagement.Core.Entities;

namespace MachineManagement.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Machine mappings
            CreateMap<Machine, MachineDto>();
            CreateMap<CreateMachineDto, Machine>();
            CreateMap<UpdateMachineDto, Machine>()
                .ForMember(dest => dest.MachineId, opt => opt.Ignore())
                .ForMember(dest => dest.MachineCode, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());

            // LogData mappings
            CreateMap<LogData, LogDataDto>();
            CreateMap<CreateLogDataDto, LogData>()
                .ForMember(dest => dest.LogTimestamp, opt => opt.MapFrom(src => src.LogTimestamp ?? DateTime.UtcNow))
                .ForMember(dest => dest.LogId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Machine, opt => opt.Ignore());

            // Command mappings
            CreateMap<Command, CommandDto>();
            CreateMap<CreateCommandDto, Command>()
                .ForMember(dest => dest.CommandId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"))
                .ForMember(dest => dest.ExecutedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Response, opt => opt.Ignore())
                .ForMember(dest => dest.ErrorMessage, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Machine, opt => opt.Ignore());
        }
    }
}