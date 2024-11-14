using Meowmentum.Server.Dotnet.Core.Entities;

namespace Meowmentum.Server.Dotnet.Business.Abstractions;

public interface ITokenService
{
    string GetToken(AppUser user);
}
