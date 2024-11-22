using Meowmentum.Server.Dotnet.Infrastructure.HelperServices.Mappings;
using Meowmentum.Server.Dotnet.Core.Entities;
using AutoMapper;

namespace Meowmentum.Server.Dotnet.Shared.Requests.Tags;

public class CreateTagRequest : IMapWith<Tag>
{
    public string Name { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CreateTagRequest, Tag>()
            .ForMember(tag => tag.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(tag => tag.UpdatedDate, opt => opt.Ignore())
            .ForMember(tag => tag.UserId, opt => opt.Ignore());
    }
}