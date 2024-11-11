using Meowmentum.Server.Dotnet.Persistence.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Extensions;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Meowmentum.Server.Dotnet.Persistence.Redis;

public class RedisCacheService(IConnectionMultiplexer redisConnection, ILogger<IRedisCacheService> logger) : IRedisCacheService
{
    private IDatabase GetDatabase(int? dbNum)
    {
        return redisConnection.GetDatabase(dbNum ?? -1);
    }
    public async Task<Result<bool>> SetAsync<T>(string key, T value, TimeSpan expiration, int? dbNum = -1, bool overrideValeIfKeyExists = false, CancellationToken ct = default)
    {
        try
        {
            var db = GetDatabase(dbNum);

            logger.LogInformation($"Setting cache key: {key} in db: {dbNum}");
            var json = value.JsonSerializeOrDefault();
            if (json == null)
            {
                logger.LogError(ResultMessages.Json.SerializationError);
                return Result.Failure<bool>(ResultMessages.Json.SerializationError);
            }

            if (!overrideValeIfKeyExists)
            {
                var exists = await db.KeyExistsAsync(key);
                if (exists)
                {
                    logger.LogInformation($"Key: {key} already exists in db: {dbNum}");
                    return Result.Success(true);
                }
            }

            var setResult = await db.StringSetAsync(key, json, expiration);
            if (!setResult)
            {
                logger.LogError(ResultMessages.RedisCache.FailToSet);
                return Result.Failure<bool>(ResultMessages.RedisCache.FailToSet);
            }

            return Result.Success(setResult);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ResultMessages.RedisCache.FailToSet);
            return Result.Failure<bool>(ResultMessages.RedisCache.FailToSet);
        }
    }

    public async Task<Result<bool>> SetAsync<T>(string key, T value, int? dbNum = -1, bool overrideValeIfKeyExists = false, CancellationToken ct = default)
    {
        try
        {
            var db = GetDatabase(dbNum);
            var json = value.JsonSerializeOrDefault();
            if (json == null)
            {
                logger.LogError(ResultMessages.Json.SerializationError);
                return Result.Failure<bool>(ResultMessages.Json.SerializationError);
            }

            if (!overrideValeIfKeyExists)
            {
                var exists = await db.KeyExistsAsync(key);
                if (exists)
                {
                    logger.LogInformation($"Key: {key} already exists in db: {dbNum}");
                    return Result.Success(true);
                }
            }

            var setResult = await db.StringSetAsync(key, json);
            if (!setResult)
            {
                logger.LogError(ResultMessages.RedisCache.FailToSet);
                return Result.Failure<bool>(ResultMessages.RedisCache.FailToSet);
            }

            return Result.Success(setResult);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ResultMessages.RedisCache.FailToSet);
            return Result.Failure<bool>(ResultMessages.RedisCache.FailToSet);
        }
    }

    public async Task<Result<T>> GetAsync<T>(string key, int? dbNum = -1, CancellationToken ct = default)
    {
        try
        {
            var db = GetDatabase(dbNum);

            logger.LogInformation($"Getting cache key: {key} from db: {dbNum}");
            var value = await db.StringGetAsync(key);
            if (!value.HasValue)
            {
                logger.LogInformation(ResultMessages.RedisCache.KeyNotFound);
                return Result.Failure<T>(ResultMessages.RedisCache.KeyNotFound);
            }

            if (value.IsNullOrEmpty)
            {
                logger.LogInformation(ResultMessages.RedisCache.NullOrEmptyValue);
                return Result.Failure<T>(ResultMessages.RedisCache.NullOrEmptyValue);
            }

            var result = value.ToString().JsonDeserializeOrDefault<T>();
            if (result == null)
            {
                logger.LogError(ResultMessages.Json.SerializationError);
                return Result.Failure<T>(ResultMessages.Json.SerializationError);
            }

            return Result.Success(result); 
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ResultMessages.RedisCache.FailToGet);
            return Result.Failure<T>(ResultMessages.RedisCache.FailToGet);
        }
    }

    public async Task<Result<bool>> ExistsAsync(string key, int? dbNum = -1, CancellationToken ct = default)
    {
        try
        {
            var db = GetDatabase(dbNum);
            var exists = await db.KeyExistsAsync(key);
            if (!exists)
            {
                return Result.Failure<bool>(ResultMessages.RedisCache.KeyNotFound);
            }

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ResultMessages.RedisCache.FailToCheck);
            return Result.Failure<bool>(ResultMessages.RedisCache.FailToCheck);
        }
    }

    public async Task<Result<bool>> RemoveAsync(string key, int? dbNum = -1, CancellationToken ct = default)
    {
        try
        {
            var db = GetDatabase(dbNum);
            var deleteResult = await db.KeyDeleteAsync(key);
            if (!deleteResult)
            {
                logger.LogWarning(ResultMessages.RedisCache.KeyNotFound);
                return Result.Success(deleteResult);
            }

            return Result.Success(deleteResult);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ResultMessages.RedisCache.FailToRemove);
            return Result.Failure<bool>(ResultMessages.RedisCache.FailToRemove);
        }
    }
}
