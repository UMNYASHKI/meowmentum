using Meowmentum.Server.Dotnet.Infrastructure.Abstractions;
using Meowmentum.Server.Dotnet.Persistence.Abstractions;
using Meowmentum.Server.Dotnet.Persistence.Redis;
using Meowmentum.Server.Dotnet.Shared.Extensions;
using Meowmentum.Server.Dotnet.Shared.Options.Redis;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace Meowmentum.Server.Dotnet.Infrastructure.HelperServices;

public class TokenBlackListManger(
    IRedisCacheService redisCacheService, 
    IOptions<TokenBlacklistDbConfig> blackListOptions,
    ILogger<ITokenBlackListManager> logger) : ITokenBlackListManager
{
    private readonly TokenBlacklistDbConfig _blackListOptions = blackListOptions.Value;

    public async Task<Result<bool>> IsTokenBlacklisted(string token, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Result.Failure<bool>(ResultMessages.User.InvalidToken);
        }

        return await redisCacheService.ExistsAsync(_blackListOptions.Prefix.Append(token), _blackListOptions.DbNumber, ct);
    }

    public async Task<Result<bool>> AddTokenToBlackList(string token, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Result.Failure<bool>(ResultMessages.User.InvalidToken);
        }

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        if (jwt == null)
        {
            logger.LogError(ResultMessages.User.InvalidToken.Append($"Token: {token}"));
            return Result.Failure<bool>(ResultMessages.User.InvalidToken);
        }

        var expiration = jwt.ValidTo - DateTime.UtcNow;
        if (expiration <= TimeSpan.Zero)
        {
            logger.LogError(ResultMessages.User.ExpiredToken.Append($"Token: {token}"));
            return Result.Failure<bool>(ResultMessages.User.ExpiredToken);
        }

        return await redisCacheService.SetAsync(_blackListOptions.Prefix.Append(token), true, expiration, _blackListOptions.DbNumber, false, ct);
    }
}
