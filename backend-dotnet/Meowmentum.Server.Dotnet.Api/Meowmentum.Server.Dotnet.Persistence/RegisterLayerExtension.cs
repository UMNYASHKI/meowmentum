using Meowmentum.Server.Dotnet.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Meowmentum.Server.Dotnet.Persistence;

public static class RegisterLayerExtension
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options
                .UseLazyLoadingProxies()
                .UseNpgsql(
                    configuration.GetConnectionString("MasterDatabase"));
        });

        services.AddRedisConfiguration(configuration);
    }
}

