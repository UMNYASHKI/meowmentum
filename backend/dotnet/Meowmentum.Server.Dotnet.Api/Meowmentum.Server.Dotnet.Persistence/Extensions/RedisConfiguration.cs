using Meowmentum.Server.Dotnet.Persistence.Abstractions;
using Meowmentum.Server.Dotnet.Persistence.Redis;
using Meowmentum.Server.Dotnet.Shared.Options.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Meowmentum.Server.Dotnet.Persistence.Extensions;

public static class RedisConfiguration
{
    public static IServiceCollection AddRedisConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var redisConnection = provider.GetRequiredService<IOptions<RedisOptions>>().Value;
            return ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = { { redisConnection.Host, redisConnection.Port } },
                Password = redisConnection.Password,
                Ssl = redisConnection.Ssl,
                ConnectTimeout = redisConnection.ConnectTimeout,
                ConnectRetry = redisConnection.ConnectRetry
            });
        });

        services.AddScoped<IRedisCacheService, RedisCacheService>();

        return services;
    }
}
