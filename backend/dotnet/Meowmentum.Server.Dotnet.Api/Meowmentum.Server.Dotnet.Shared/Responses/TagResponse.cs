using AutoMapper;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Infrastructure.HelperServices.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Shared.Responses;

public class TagResponse : IMapWith<Tag>
{
    public long Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public long UserId { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Tag, TagResponse>();
    }
}
