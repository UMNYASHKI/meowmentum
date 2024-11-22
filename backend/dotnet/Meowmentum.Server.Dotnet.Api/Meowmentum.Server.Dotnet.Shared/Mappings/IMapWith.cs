using AutoMapper;

namespace Meowmentum.Server.Dotnet.Infrastructure.HelperServices.Mappings;

public interface IMapWith<T>
{
    void Mapping(Profile profile)
    => profile.CreateMap(typeof(T), GetType());
}
