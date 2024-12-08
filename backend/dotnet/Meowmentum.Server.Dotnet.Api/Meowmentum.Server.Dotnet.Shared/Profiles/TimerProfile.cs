using AutoMapper;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Requests;
namespace Meowmentum.Server.Dotnet.Shared.Profiles;

public class TimerProfile : Profile
{
    public TimerProfile()
    {
        CreateMap<TimerUpdateRequest, TimeInterval>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
