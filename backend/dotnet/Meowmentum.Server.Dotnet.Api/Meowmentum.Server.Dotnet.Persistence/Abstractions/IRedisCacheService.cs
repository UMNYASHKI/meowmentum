using Meowmentum.Server.Dotnet.Shared.Results;

namespace Meowmentum.Server.Dotnet.Persistence.Abstractions;
public interface IRedisCacheService
{
    Task<Result<bool>> ExistsAsync(string key, int? dbNum = -1, CancellationToken ct = default);
    Task<Result<T>> GetAsync<T>(string key, int? dbNum = -1, CancellationToken ct = default);
    Task<Result<bool>> RemoveAsync(string key, int? dbNum = -1, CancellationToken ct = default);
    Task<Result<bool>> SetAsync<T>(string key, T value, TimeSpan expiration, int? dbNum = -1, bool overrideValeIfKeyExists = false, CancellationToken ct = default);
    Task<Result<bool>> SetAsync<T>(string key, T value, int? dbNum = -1, bool overrideValeIfKeyExists = false, CancellationToken ct = default);
}