using Meowmentum.Server.Dotnet.Shared.Results;

namespace Meowmentum.Server.Dotnet.Infrastructure.Abstractions;

public interface ITokenBlackListManager
{
    Task<Result<bool>> AddTokenToBlackList(string token, CancellationToken ct = default);
    Task<Result<bool>> IsTokenBlacklisted(string token, CancellationToken ct = default);
}
