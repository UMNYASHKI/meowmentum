using AutoMapper;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Infrastructure.HelperServices.Mappings;

namespace Meowmentum.Server.Dotnet.Shared.Requests.Tags;

public class UpdateTagRequest : IMapWith<Tag>
{
    public string Name { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<UpdateTagRequest, Tag>()
            .ForMember(tag => tag.UpdatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
