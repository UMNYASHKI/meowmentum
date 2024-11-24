using AutoMapper;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Requests;
using Meowmentum.Server.Dotnet.Shared.Responses;

namespace Meowmentum.Server.Dotnet.Shared.Profiles;

public class TagProfile : Profile
{
    public TagProfile()
    {
        CreateMap<TagRequest, Tag>()
            .ForMember(tag => tag.CreatedDate, opt => opt.Ignore())
            .ForMember(tag => tag.UpdatedDate, opt => opt.Ignore())
            .ForMember(tag => tag.UserId, opt => opt.Ignore());

        CreateMap<Tag, TagResponse>();
    }
}
