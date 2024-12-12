using AutoMapper;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Requests;
using Meowmentum.Server.Dotnet.Shared.Responses;
namespace Meowmentum.Server.Dotnet.Shared.Profiles;

public class TimerProfile : Profile
{
    public TimerProfile()
    {
        CreateMap<TimerUpdateRequest, TimeInterval>()
    .ForMember(dest => dest.StartTime, opt => opt.Condition((src, dest, srcMember) => srcMember != null))
    .ForMember(dest => dest.EndTime, opt => opt.Condition((src, dest, srcMember) => srcMember != null))
    .ForMember(dest => dest.Description, opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<TimeInterval, TimeIntervalResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.TaskId))
                .ReverseMap();
    }
}
